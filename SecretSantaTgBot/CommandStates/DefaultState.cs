using System.Text;

using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.CommandStates;

public class DefaultState : CommandStateBase
{
    public const string TITLE = "default";

    private readonly Dictionary<string, CommandInfo> _commands;
    private readonly Dictionary<string, CommandStateBase> _innerStates;

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;

    public DefaultState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;

        _commands = new List<CommandInfo> {
            new("/start", "START", CommandStart)
            {
                ShowHelp = false
            },
            new("/help", _msgDict[_lang].CommandHelp, CommandHelp),
            new("/show_rooms", _msgDict[_lang].CommandShowRooms, CommandShowRooms),
            new("/create_room", _msgDict[_lang].CommandCreateRoom, CommandCreateRoom),
            new("/join_room", _msgDict[_lang].CommandJoinRoom, CommandJoinRoom),
            new("/select_room", _msgDict[_lang].CommandSelectRoom, CommandSelectRoom),
            new("/delete_room", _msgDict[_lang].CommandDeleteRoom, CommandDeleteRoom),
        }.ToDictionary(x => x.Command);

        _innerStates = new()
        {
            [RoomSelectState.TITLE] = new RoomSelectState(csm),
            [RoomDeleteState.TITLE] = new RoomDeleteState(csm),
        };
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        var text = msg.Text!;

        if (!text.StartsWith('/'))
        {
            var states = user.CurrentState is not null
                ? NameParser.ParseArgs(user.CurrentState!)
                : [];
            if (states.Length > 1)
            {
                if (_innerStates.TryGetValue(states[1], out var innserState))
                    return innserState.OnMessage(msg, user);
            }

            return OnCommandError(msg.Chat);
        }

        var words = text.Split(' ').Where(x => x.Length > 0).ToArray();
        var command = words[0];
        var args = words.Length > 1
            ? words[1..]
            : [];

        var cmd = _commands.GetValueOrDefault(command);
        if (cmd is null)
            return OnCommandError(msg.Chat);

        user.CurrentState = default;
        _db.Users.Update(user);

        return cmd.Callback.Invoke(msg.Chat, user, args);
    }

    private Task CommandStart(Chat chat, UserTg user, string[] args)
        => CommandHelp(chat, user, args);

    private Task CommandHelp(Chat chat, UserTg user, string[] args)
    {
        var msg = MessageBuilder.GetHelpMessage(_commands.Values, _msgDict[_lang]);

        return _bot.SendMessage(chat, msg,
                parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
    }

    private Task CommandCreateRoom(Chat chat, UserTg user, string[] args)
    {
        UpdateUserState(chat.Id, RoomCreateState.TITLE);
        return _bot.SendMessage(chat, _msgDict[_lang].RoomCreationEnterTitle,
                parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
    }

    private Task CommandJoinRoom(Chat chat, UserTg user, string[] args)
    {
        if (args is not { Length: > 0 })
            return OnCommandError(chat);

        var roomId = new Guid(args[0]);
        var room = _db.Rooms.FindById(roomId);
        if (room is null)
            return _bot.SendMessage(chat, _msgDict[_lang].RoomDoesntExist);

        var newState = NameParser.JoinArgs(RegistrationState.TITLE, roomId);
        UpdateUserState(user.Id, newState);

        return _bot.SendMessage(chat,
            _msgDict[_lang].EnterRealName,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private Task CommandSelectRoom(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return _bot.SendMessage(chat, _msgDict[_lang].ZeroRooms);

        var newState = NameParser.JoinArgs(TITLE, RoomSelectState.TITLE);
        UpdateUserState(user.Id, newState);

        var buttons = user.AvailableRooms.Select(x => $"{x.Title} {x.Id}").ToArray();
        return _bot.SendMessage(chat,
            _msgDict[_lang].ChooseRoom,
            parseMode: ParseMode.Html,
            replyMarkup: buttons);
    }

    private Task CommandDeleteRoom(Chat chat, UserTg user, string[] args)
    {
        var rooms = user.AvailableRooms?.Where(x => x.Admin.Id == user.Id).ToList();

        if (rooms is not { Count: > 0 })
            return _bot.SendMessage(chat, _msgDict[_lang].ZeroRooms);

        var newState = NameParser.JoinArgs(TITLE, RoomDeleteState.TITLE);
        UpdateUserState(user.Id, newState);

        var buttons = rooms.Select(x => $"{x.Title} {x.Id}").ToArray();
        return _bot.SendMessage(chat,
            _msgDict[_lang].ChooseRoom,
            parseMode: ParseMode.Html,
            replyMarkup: buttons);
    }

    private Task CommandShowRooms(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return _bot.SendMessage(chat, _msgDict[_lang].ZeroRooms);

        var strBldr = new StringBuilder();
        strBldr.AppendLine(_msgDict[_lang].RoomsList);
        strBldr.AppendLine();

        foreach (var room in user.AvailableRooms)
        {
            var adminText = room.Admin.Id == user.Id
                ? " - admin"
                : string.Empty;

            strBldr.AppendLine($" ► <i>{room.Id}{adminText}</i>");
            strBldr.AppendLine($"   <b>{room.Title}</b>");
            strBldr.AppendLine($"   {room.PartyDescription}");
            strBldr.AppendLine();
        }

        return _bot.SendMessage(chat, strBldr.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private Task OnCommandError(Chat chat)
        => _bot.SendMessage(chat, _msgDict[_lang].CommandError);

    private void UpdateUserState(long userId, string state)
    {
        var user = _db.Users.FindById(userId);
        user.CurrentState = state;
        _db.Users.Update(user);
    }
}
