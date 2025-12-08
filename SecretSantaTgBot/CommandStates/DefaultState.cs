using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.CommandStates;

public class DefaultState : CommandStateBase
{
    public const string TITLE = "default";

    private readonly Dictionary<string, CommandInfo> _commands;
    private readonly Dictionary<string, CommandStateBase> _innerStates;

    private readonly SantaDatabase _db;
    private readonly MessageBrokerService _csm;
    private readonly NotificationService _notifyService;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public DefaultState(MessageBrokerService csm)
    {
        _csm = csm;
        _db = csm.DB;
        _notifyService = csm.NotifyService;

        _commands = new List<CommandInfo> {
            new("/start", "START", CommandStart)
            {
                ShowHelp = false
            },
            new("/help", Msgs.CommandHelp, CommandHelp),
            new("/select_room", Msgs.CommandSelectRoom, CommandSelectRoom),
            new("/create_room", Msgs.CommandCreateRoom, CommandCreateRoom),
            new("/delete_room", Msgs.CommandDeleteRoom, CommandDeleteRoom),
            new("/show_rooms", Msgs.CommandShowRooms, CommandShowRooms)
        }.ToDictionary(x => x.Command);

        _innerStates = new()
        {
            [RoomSelectState.TITLE] = new RoomSelectState(csm),
            [RoomDeleteState.TITLE] = new RoomDeleteState(csm),
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

    private void UpdateUserState(UserTg user, string state)
    {
        user.CurrentState = state;
        _db.Users.Update(user);
    }



    private Task CommandStart(Chat chat, UserTg user, string[] args)
    {
        if (args.Length == 0)
            return CommandHelp(chat, user, args);

        var roomId = new Guid(args[0]);
        var room = _db.Rooms.FindById(roomId);
        if (room == null)
            return _notifyService.SendErrorMessage(chat.Id, Msgs.RoomDoesntExist);

        user.SelectedRoom = room;

        if (user.AvailableRooms.Any(x => x.Id == room.Id))
            return _csm.UpdateAfterStatusChanged(user);

        var newState = NameParser.JoinArgs(RegistrationState.TITLE, room.Id);
        UpdateUserState(user, newState);

        return _notifyService.SendMessage(user.Id, Msgs.EnterRealName);
    }

    private Task CommandHelp(Chat chat, UserTg user, string[] args)
    {
        var msg = MessageBuilder.BuildHelpMessage(_commands.Values, true);
        return _notifyService.SendMessage(chat.Id, msg);
    }



    private Task CommandCreateRoom(Chat chat, UserTg user, string[] args)
    {
        UpdateUserState(user, RoomCreateState.TITLE);
        return _notifyService.SendMessage(chat.Id, Msgs.RoomCreationEnterTitle);
    }

    private Task CommandSelectRoom(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return _notifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        var newState = NameParser.JoinArgs(TITLE, RoomSelectState.TITLE);
        UpdateUserState(user, newState);

        var buttons = user.AvailableRooms.Select(x => $"{x.Title} {x.Id}").ToArray();
        return _notifyService.SendMessage(chat.Id, Msgs.ChooseRoom, buttons!);
    }

    private Task CommandDeleteRoom(Chat chat, UserTg user, string[] args)
    {
        var rooms = user.AvailableRooms?.Where(x => x.Admin.Id == user.Id).ToList();

        if (rooms is not { Count: > 0 })
            return _notifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        var newState = NameParser.JoinArgs(TITLE, RoomDeleteState.TITLE);
        UpdateUserState(user, newState);

        var buttons = rooms.Select(x => $"{x.Title} {x.Id}").ToArray();
        return _notifyService.SendMessage(chat.Id, Msgs.ChooseRoom, buttons!);
    }

    private Task CommandShowRooms(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return _notifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        var message = MessageBuilder.BuildRoomsInfoMessage(user);
        return _notifyService.SendMessage(chat.Id, message);
    }
}
