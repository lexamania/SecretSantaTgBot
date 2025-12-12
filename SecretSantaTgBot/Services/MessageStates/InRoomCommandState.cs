using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.DefaultStates;
using SecretSantaTgBot.Services.MessageStates.InRoomStates;
using SecretSantaTgBot.Storage.Models;
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
            new("/start_santa", Msgs.StartSanta, CommandStartSecretSanta)
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
            [LEAVE_TITLE] = new ConfirmationState(csm, Title, LEAVE_TITLE, CommandLeaveRoomConfirmation),
        };
    }

    public override async Task<bool> OnMessage(Message msg, UserTg user)
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



    private bool IsAdmin(UserTg user)
        => user.SelectedRoom!.Admin.Id == user.Id;

    private Participant GetParticipantById(UserTg user, long userId)
        => user.SelectedRoom!.Users.First(x => x.Id == userId);

    private Participant GetMeAsParticipant(UserTg user)
        => GetParticipantById(user, user.Id);



    private Task CommandBack(Chat chat, UserTg user, string[] args)
    {
        user.SelectedRoom = default;
        DB.Users.Update(user);
        return Csm.UpdateAfterStatusChanged(user);
    }

    private Task CommandShowRoomInfo(Chat chat, UserTg user, string[] args)
    {
        var message = MessageBuilder.BuildRoomInfoMessage(user.SelectedRoom!);
        return NotifyService.SendMessage(chat.Id, message);
    }

    private Task CommandLeaveRoom(Chat chat, UserTg user, string[] args)
    {
        return IsAdmin(user)
            ? NotifyService.SendErrorMessage(chat.Id, Msgs.AdminCantLeaveRoom)
            : _innerStates[LEAVE_TITLE].StartState(user, args);
    }

    private async Task CommandLeaveRoomConfirmation(Chat chat, UserTg user, string[] args)
    {
        var room = user.SelectedRoom!;
        var part = GetMeAsParticipant(user);

        user.SelectedRoom = default;
        user.AvailableRooms.Remove(room);
        room.Users.Remove(part);

        DB.Users.Update(user);
        DB.Rooms.Update(room);

        var message = MessageBuilder.BuildLeaveMessage(part);
        await NotifyService.NotifyEveryoneInRoom(room, message);
        await NotifyService.SendMessage(chat.Id, Msgs.UserLeavedRoom);
    }



    private Task CommandShowMe(Chat chat, UserTg user, string[] args)
    {
        var target = GetMeAsParticipant(user);
        return ShowUserInfo(user.Id, target,
            Msgs.UserWishesList,
            Msgs.UserHaveZeroWishes);
    }

    private Task CommandShowTarget(Chat chat, UserTg user, string[] args)
    {
        var me = GetMeAsParticipant(user);
        if (me.TargetUserId is null)
            return NotifyService.SendErrorMessage(chat.Id, Msgs.SecretSantaStillOffline);

        var target = GetParticipantById(user, me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            Msgs.TargetWishesList,
            Msgs.TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, Participant target, string header, string emptyMsg)
    {
        var message = MessageBuilder.BuildUserInfoMessage(header, target);
        await NotifyService.SendMessage(chatId, message);

        foreach (var wish in target.Wishes.Where(x => x.Images is { Count: > 0 }))
            await NotifyService.SendImages(chatId, wish.Images, wish.Message);
    }



    private Task CommandStartWishes(Chat chat, UserTg user, string[] args)
        => _innerStates[InRoomWishesState.TITLE].StartState(user, args);

    private Task CommandClearWishes(Chat chat, UserTg user, string[] args)
    {
        var me = GetMeAsParticipant(user);
        me.Wishes.Clear();
        DB.Rooms.Update(user.SelectedRoom!);

        return NotifyService.SendMessage(chat.Id, Msgs.UserWishesCleared);
    }



    private Task CommandUpdateRoom(Chat chat, UserTg user, string[] args)
    {
        return !IsAdmin(user)
            ? NotifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights)
            : _innerStates[InRoomUpdateState.TITLE].StartState(user, args);
    }

    private Task CommandStartSecretSanta(Chat chat, UserTg user, string[] args)
    {
        var room = user.SelectedRoom!;
        if (!IsAdmin(user))
            return NotifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights);

        var participants = room.Users;
        if (participants.Count < 2)
            return NotifyService.SendErrorMessage(chat.Id, Msgs.NotEnoughParticipants);

        room.Users = ShuffleTargets(room.Users);
        room.IsPlayed = true;
        DB.Rooms.Update(room);

        return NotifyEveryoneTheirTarget(room);
    }

    private List<Participant> ShuffleTargets(List<Participant> participants)
    {
        var pArray = participants.ToArray();
        var targetListIds = RandomExtension.GetShuffledUniqueIndexRange(pArray.Length);

        for (int i = 0; i < pArray.Length; ++i)
            pArray[i].TargetUserId = pArray[targetListIds[i]].Id;

        return participants;
    }

    private Task NotifyEveryoneTheirTarget(PartyRoom room)
    {
        var result = new List<(long Id, string Msg)>();

        foreach (var p in room.Users)
        {
            var target = room.Users.First(x => x.Id == p.TargetUserId);
            var message = MessageBuilder.BuildTargetMessage(room, target);
            result.Add((target.Id, message));
        }

        return NotifyService.NotifyEveryone(result);
    }
}
