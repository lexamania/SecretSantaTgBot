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

    public InRoomCommandState(MessageBrokerService csm) : base(csm, TITLE)
    {
        var commands = new List<CommandInfo> {
            new("/show_room_info", Msgs.CommandShowRoomInfo, CommandShowRoomInfo),
            new("/leave_room", Msgs.CommandLeaveRoom, CommandLeaveRoom)
            {
                Access = AccessRights.NotForAdmin
            },
            new("/show_me", Msgs.CommandShowMe, CommandShowMe),
            new("/show_my_target", Msgs.CommandShowTarget, CommandShowTarget),
            new("/start_wishes", Msgs.CommandStartWishes, CommandStartWishes),
            new("/clear_wishes", Msgs.CommandClearWishes, CommandClearWishes),
            new("/update_room_info", Msgs.CommandUpdateRoom, CommandUpdateRoom)
            {
                Access = AccessRights.Admin
            },
            new("/notify_everyone", Msgs.CommandNotifyEveryone, CommandNotifyEveryone)
            {
                Access = AccessRights.Admin
            },
            new("/start_santa", Msgs.CommandStartSanta, CommandStartSecretSanta)
            {
                Access = AccessRights.Admin
            },
            new("/back", Msgs.CommandBack, CommandBack),
        };

        foreach(var command in commands)
            Commands.Add(command.Command, command);

        _innerStates = new()
        {
            [InRoomWishesState.TITLE] = new InRoomWishesState(csm, Title),
            [InRoomUpdateState.TITLE] = new InRoomUpdateState(csm, Title),
            [InRoomNotifyEveryoneState.TITLE] = new InRoomNotifyEveryoneState(csm, Title),
            [LEAVE_TITLE] = new ConfirmationState(csm, Title, LEAVE_TITLE, CommandLeaveRoomConfirmation),
        };
    }

    public override async Task<bool> OnMessage(Message msg, UserEntity user)
    {
        if (MessageParser.HasNewState(_innerStates, user.CurrentState!, Title, out var innerState))
        {
            if (await innerState!.OnMessage(msg, user))
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
        DB.UserDirectory.UpdateWithClearRoom(user);
        return Csm.UpdateAfterStatusChanged(user);
    }

    private Task CommandShowRoomInfo(Chat chat, UserEntity user, string[] args)
    {
        var message = MessageBuilder.BuildRoomInfoMessage(user.SelectedRoom!);
        return NotifyService.SendMessage(chat.Id, message);
    }

    private Task CommandLeaveRoom(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? NotifyService.SendErrorMessage(chat.Id, Msgs.AdminCantLeaveRoom)
            : _innerStates[LEAVE_TITLE].StartState(user, args);
    }

    private async Task CommandLeaveRoomConfirmation(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room);

        DB.LeaveRoom(user, room);

        var message = MessageBuilder.BuildLeaveMessage(participant);
        await NotifyService.NotifyEveryoneInRoom(room, message);
        await NotifyService.SendMessage(chat.Id, Msgs.UserLeavedRoom);
    }



    private Task CommandShowMe(Chat chat, UserEntity user, string[] args)
    {
        var target = user.GetAsParticipant()!;
        return ShowUserInfo(user.Id, target,
            Msgs.UserWishesList,
            Msgs.UserHaveZeroWishes);
    }

    private Task CommandShowTarget(Chat chat, UserEntity user, string[] args)
    {
        var me = user.GetAsParticipant()!;
        if (me.TargetUserId is null)
            return NotifyService.SendErrorMessage(chat.Id, Msgs.SecretSantaStillOffline);

        var room = user.SelectedRoom!;
        var target = room.Users.First(x => x.Id == me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            Msgs.TargetWishesList,
            Msgs.TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, ParticipantEntity target, string header, string emptyMsg)
    {
        var message = MessageBuilder.BuildUserInfoMessage(header, target);
        await NotifyService.SendMessage(chatId, message);

        foreach (var wish in target.Wishes.Where(x => x.Images is { Count: > 0 }))
            await NotifyService.SendImages(chatId, wish.Images, wish.Message);
    }



    private Task CommandStartWishes(Chat chat, UserEntity user, string[] args)
        => _innerStates[InRoomWishesState.TITLE].StartState(user, args);

    private Task CommandClearWishes(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;
        participant.Wishes.Clear();
        DB.RoomDirectory.Update(room);

        return NotifyService.SendMessage(chat.Id, Msgs.UserWishesCleared);
    }



    private Task CommandUpdateRoom(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? NotifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights)
            : _innerStates[InRoomUpdateState.TITLE].StartState(user, args);
    }

    private Task CommandNotifyEveryone(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? NotifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights)
            : _innerStates[InRoomNotifyEveryoneState.TITLE].StartState(user, args);
    }

    private Task CommandStartSecretSanta(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;

        if (!user.IsAdmin())
            return NotifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights);

        if (room.IsPlayed)
            return NotifyService.SendErrorMessage(chat.Id, Msgs.SecretSantaWasPlayed);

        var participants = room.Users;
        if (participants.Count < 2)
            return NotifyService.SendErrorMessage(chat.Id, Msgs.NotEnoughParticipants);

        room.Users = ShuffleTargets(room.Users);
        room.IsPlayed = true;
        DB.RoomDirectory.Update(room);

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

        return NotifyService.NotifyEveryone(result);
    }
}
