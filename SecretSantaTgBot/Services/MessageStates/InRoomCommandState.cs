using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.DefaultStates;
using SecretSantaTgBot.Services.MessageStates.InRoomStates;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates;

public class InRoomCommandState : MessageStateBase
{
    public const string TITLE = "in_room";
    private const string LEAVE_TITLE = "leave_confirmation";

    private readonly Dictionary<string, MessageStateBase> _innerStates;

    public InRoomCommandState(MessageStateServiceContext context) : base(context, TITLE)
    {
        var commands = new List<CommandModel> {
            new("/show_room_info", Localization.CommandShowRoomInfo, CommandShowRoomInfo),
            new("/leave_room", Localization.CommandLeaveRoom, CommandLeaveRoom)
            {
                Access = AccessRights.NotForAdmin
            },
            new("/show_me", Localization.CommandShowMe, CommandShowMe),
            new("/show_my_target", Localization.CommandShowTarget, CommandShowTarget),
            new("/start_wishes", Localization.CommandStartWishes, CommandStartWishes),
            new("/clear_wishes", Localization.CommandClearWishes, CommandClearWishes),
            new("/update_room_info", Localization.CommandUpdateRoom, CommandUpdateRoom)
            {
                Access = AccessRights.Admin
            },
            new("/notify_everyone", Localization.CommandNotifyEveryone, CommandNotifyEveryone)
            {
                Access = AccessRights.Admin
            },
            new("/start_santa", Localization.CommandStartSanta, CommandStartSecretSanta)
            {
                Access = AccessRights.Admin
            },
            new("/back", Localization.CommandBack, CommandBack),
        };

        foreach(var command in commands)
            Commands.Add(command.Command, command);

        _innerStates = new()
        {
            [InRoomWishesState.TITLE] = new InRoomWishesState(context, StatePath),
            [InRoomUpdateState.TITLE] = new InRoomUpdateState(context, StatePath),
            [InRoomNotifyEveryoneState.TITLE] = new InRoomNotifyEveryoneState(context, StatePath),
            [LEAVE_TITLE] = new ConfirmationState(context, StatePath, LEAVE_TITLE, CommandLeaveRoomConfirmation),
        };
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (StateUtil.HasNewState(user.CurrentState!, StatePath, _innerStates, out var innerState))
        {
            if (await innerState!.ProcessMessage(msg, user))
                return true;
        }

        if (MessageParser.IsCommand(msg, out var command, out var commandArgs))
        {
            if (!Commands.TryGetValue(command!, out var cmd))
                return false;
        
            await cmd.Callback.Invoke(msg.Chat, user, commandArgs!);
            return true;
        }

        return false;
    }



    private Task CommandBack(Chat chat, UserEntity user, string[] args)
    {
        Database.UserDirectory.UpdateWithClearRoom(user);
        return MessageHandler.SendHelpMenu(user);
    }

    private Task CommandShowRoomInfo(Chat chat, UserEntity user, string[] args)
    {
        var message = MessageBuilder.BuildRoomInfoMessage(user.SelectedRoom!);
        return Notifications.SendMessage(chat.Id, message);
    }

    private Task CommandLeaveRoom(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? Notifications.SendErrorMessage(chat.Id, Localization.AdminCantLeaveRoom)
            : _innerStates[LEAVE_TITLE].PrepareState(user, args);
    }

    private async Task CommandLeaveRoomConfirmation(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;

        Database.RemoveUserFromRoom(user, room);

        var message = MessageBuilder.BuildLeaveMessage(participant);
        await Notifications.NotifyEveryoneInRoom(room, message);
        await Notifications.SendMessage(chat.Id, Localization.UserLeavedRoom);
    }



    private Task CommandShowMe(Chat chat, UserEntity user, string[] args)
    {
        var target = user.GetAsParticipant()!;
        return ShowUserInfo(user.Id, target,
            Localization.UserWishesList,
            Localization.UserHaveZeroWishes);
    }

    private Task CommandShowTarget(Chat chat, UserEntity user, string[] args)
    {
        var me = user.GetAsParticipant()!;
        if (me.TargetUserId is null)
            return Notifications.SendErrorMessage(chat.Id, Localization.SecretSantaStillOffline);

        var room = user.SelectedRoom!;
        var target = room.Users.First(x => x.Id == me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            Localization.TargetWishesList,
            Localization.TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, ParticipantEntity target, string header, string emptyMsg)
    {
        var message = MessageBuilder.BuildUserInfoMessage(header, target);
        await Notifications.SendMessage(chatId, message);

        foreach (var wish in target.Wishes.Where(x => x.Images is { Count: > 0 }))
            await Notifications.SendImages(chatId, wish.Images, wish.Message);
    }



    private Task CommandStartWishes(Chat chat, UserEntity user, string[] args)
        => _innerStates[InRoomWishesState.TITLE].PrepareState(user, args);

    private Task CommandClearWishes(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;
        participant.Wishes.Clear();
        Database.RoomDirectory.Update(room);

        return Notifications.SendMessage(chat.Id, Localization.UserWishesCleared);
    }



    private Task CommandUpdateRoom(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? Notifications.SendErrorMessage(chat.Id, Localization.NeedAdminRights)
            : _innerStates[InRoomUpdateState.TITLE].PrepareState(user, args);
    }

    private Task CommandNotifyEveryone(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? Notifications.SendErrorMessage(chat.Id, Localization.NeedAdminRights)
            : _innerStates[InRoomNotifyEveryoneState.TITLE].PrepareState(user, args);
    }

    private Task CommandStartSecretSanta(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;

        if (!user.IsAdmin())
            return Notifications.SendErrorMessage(chat.Id, Localization.NeedAdminRights);

        if (room.IsPlayed)
            return Notifications.SendErrorMessage(chat.Id, Localization.SecretSantaWasPlayed);

        var participants = room.Users;
        if (participants.Count < 2)
            return Notifications.SendErrorMessage(chat.Id, Localization.NotEnoughParticipants);

        room.Users = ShuffleTargets(room.Users);
        room.IsPlayed = true;
        Database.RoomDirectory.Update(room);

        return NotifyEveryoneTheirTarget(room);
    }

    private List<ParticipantEntity> ShuffleTargets(List<ParticipantEntity> participants)
    {
        var pArray = participants.ToArray();
        var targetListIds = RandomExtensions.GetShuffledUniqueIndexRange(pArray.Length);

        for (int i = 0; i < pArray.Length; ++i)
            pArray[i].TargetUserId = pArray[targetListIds[i]].Id;

        return participants;
    }

    private Task NotifyEveryoneTheirTarget(PartyRoomEntity room)
    {
        var result = new List<(long Id, string Msg)>();

        foreach (var p in room.Users)
        {
            var target = room.Users.First(x => x.Id == p.TargetUserId);
            var message = MessageBuilder.BuildTargetMessage(room, target);
            result.Add((p.Id, message));
        }

        return Notifications.NotifyEveryone(result);
    }
}
