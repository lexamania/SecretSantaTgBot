using SecretSantaTgBot.CommandStates;
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot;

public class InRoomCommandState : CommandStateBase
{
    public const string TITLE = "in_room";

    private readonly Dictionary<string, CommandInfo> _commands;
    private readonly Dictionary<string, CommandStateBase> _innerStates;

    private readonly MessageBrokerService _csm;
    private readonly SantaDatabase _db;
    private readonly NotificationService _notifyService;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public InRoomCommandState(MessageBrokerService csm)
    {
        _csm = csm;
        _db = csm.DB;
        _notifyService = _csm.NotifyService;

        _commands = new List<CommandInfo> {
            new("/help", Msgs.CommandHelp, CommandHelp),
            new("/show_room_info", Msgs.CommandShowRoomInfo, CommandShowRoomInfo),
            new("/leave_room", Msgs.CommandLeaveRoom, CommandLeaveRoom)
            {
                Access = AccessRights.NotForAdmin
            },
            new("/show_me", Msgs.CommandShowMe, CommandShowMe),
            new("/show_my_target", Msgs.CommandShowTarget, CommandShowTarget),
            new("/start_wishes", Msgs.CommandStartWishes, CommandStartWishes),
            new("/stop_wishes", Msgs.CommandStopWishes, CommandStopWishes),
            new("/clear_wishes", Msgs.CommandClearWishes, CommandClearWishes),
            new("/start_santa", Msgs.StartSanta, CommandStartSecretSanta)
            {
                Access = AccessRights.Admin
            },
            new("/back", Msgs.CommandBack, CommandBack),
        }.ToDictionary(x => x.Command);

        _innerStates = new()
        {
            [InRoomWishesState.TITLE] = new InRoomWishesState(csm)
        };
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        if (msg.Type != MessageType.Text || !msg.Text!.StartsWith('/'))
        {
            var states = user.CurrentState is not null
                ? NameParser.ParseArgs(user.CurrentState!)
                : [];
            if (states.Length > 1)
            {
                if (_innerStates.TryGetValue(states[1], out var innserState))
                    return innserState.OnMessage(msg, user);
            }

            return _notifyService.SendErrorCommandMessage(msg.Chat.Id);
        }

        var text = msg.Text!.Trim();
        var words = NameParser.ParseArgs(text);
        var command = words[0];
        var args = words.Length > 1
            ? words[1..]
            : [];

        var cmd = _commands.GetValueOrDefault(command);
        if (cmd is null)
            return _notifyService.SendErrorCommandMessage(msg.Chat.Id);

        user.CurrentState = default;
        _db.Users.Update(user);

        return cmd.Callback.Invoke(msg.Chat, user, args);
    }




    private void UpdateUserState(UserTg user, string? state)
    {
        user.CurrentState = state;
        _db.Users.Update(user);
    }

    private bool IsAdmin(UserTg user)
        => user.SelectedRoom!.Admin.Id == user.Id;

    private Participant GetParticipantById(UserTg user, long userId)
        => user.SelectedRoom!.Users.First(x => x.Id == userId);

    private Participant GetMeAsParticipant(UserTg user)
        => GetParticipantById(user, user.Id);



    private Task CommandHelp(Chat chat, UserTg user, string[] args)
    {
        var msg = MessageBuilder.BuildHelpMessage(_commands.Values, IsAdmin(user));
        return _notifyService.SendMessage(chat.Id, msg);
    }

    private Task CommandBack(Chat chat, UserTg user, string[] args)
    {
        user.SelectedRoom = default;
        _db.Users.Update(user);

        return _csm.UpdateAfterStatusChanged(user);
    }

    private async Task CommandLeaveRoom(Chat chat, UserTg user, string[] args)
    {
        if (IsAdmin(user))
        {
            await _notifyService.SendErrorMessage(chat.Id, Msgs.AdminCantLeaveRoom);
            return;
        }

        var room = user.SelectedRoom!;
        var part = GetMeAsParticipant(user);

        user.SelectedRoom = default;
        user.AvailableRooms.Remove(room);
        room.Users.Remove(part);

        _db.Users.Update(user);
        _db.Rooms.Update(room);

        var message = MessageBuilder.BuildLeaveMessage(part);
        await _notifyService.NotifyEveryone(room, message);
        await _notifyService.SendMessage(chat.Id, Msgs.UserLeavedRoom);
    }

    private Task CommandShowRoomInfo(Chat chat, UserTg user, string[] args)
    {
        var message = MessageBuilder.BuildRoomInfoMessage(user.SelectedRoom!);
        return _notifyService.SendMessage(chat.Id, message);
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
            return _notifyService.SendErrorMessage(chat.Id, Msgs.SecretSantaStillOffline);

        var target = GetParticipantById(user, me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            Msgs.TargetWishesList,
            Msgs.TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, Participant target, string header, string emptyMsg)
    {
        var message = MessageBuilder.BuildUserInfoMessage(header, target);
        await _notifyService.SendMessage(chatId, message);

        foreach (var wish in target.Wishes.Where(x => x.Images is { Count: > 0 }))
            await _notifyService.SendImages(chatId, wish.Images, wish.Message);
    }



    private Task CommandStartWishes(Chat chat, UserTg user, string[] args)
    {
        var states = NameParser.JoinArgs(TITLE, InRoomWishesState.TITLE);
        UpdateUserState(user, states);

        return _notifyService.SendMessage(chat.Id, Msgs.UserStartWishes);
    }

    private async Task CommandStopWishes(Chat chat, UserTg user, string[] args)
    {
        UpdateUserState(user, default);

        await _notifyService.SendMessage(chat.Id, Msgs.UserStopWishes);
        await _csm.UpdateAfterStatusChanged(user);
    }

    private Task CommandClearWishes(Chat chat, UserTg user, string[] args)
    {
        var me = GetMeAsParticipant(user);
        me.Wishes.Clear();
        _db.Rooms.Update(user.SelectedRoom!);

        return _notifyService.SendMessage(chat.Id, Msgs.UserWishesCleared);
    }



    private Task CommandStartSecretSanta(Chat chat, UserTg user, string[] args)
    {
        var room = user.SelectedRoom!;
        if (room.Admin.Id != user.Id)
            return _notifyService.SendErrorMessage(chat.Id, Msgs.NeedAdminRights);

        var participants = room.Users;
        if (participants.Count < 2)
            return _notifyService.SendErrorMessage(chat.Id, Msgs.NotEnoughParticipants);

        room.Users = ShuffleTargets(room.Users);
        room.IsPlayed = true;
        _db.Rooms.Update(room);

        return _notifyService.NotifyEveryoneTheirTarget(room);
    }

    private List<Participant> ShuffleTargets(List<Participant> participants)
    {
        var pArray = participants.ToArray();
        var targetListIds = RandomExtension.GetShuffledUniqueIndexRange(pArray.Length);

        for (int i = 0; i < pArray.Length; ++i)
            pArray[i].TargetUserId = pArray[targetListIds[i]].Id;

        return participants;
    }
}
