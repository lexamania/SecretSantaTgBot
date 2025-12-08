using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.CommandStates;

public class RoomCreateState : CommandStateBase
{
    public const string TITLE = "room_creation";

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public RoomCreateState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;
        _csm = csm;
    }

    public override async Task OnMessage(Message msg, UserTg user)
    {
        var stateArgs = NameParser.ParseArgs(user.CurrentState!);

        var roomId = stateArgs.Length > 1 ? stateArgs[1] : null;
        var helpMessage = roomId == null
            ? _msgDict[_lang].RoomCreationEnterTitle
            : _msgDict[_lang].RoomCreationEnterDescription;

        if (msg.Text is not { Length: > 0 } || msg.Text.StartsWith('/'))
        {
            await _bot.SendMessage(msg.Chat, $"{_msgDict[_lang].CommandError}\n\n{helpMessage}");
            return;
        }

        var text = msg.Text!.Trim();
        var task = roomId == null
            ? OnTitleEnter(user, text)
            : OnDescriptionEnter(user, text, roomId);

        await task;
    }

    private Task OnTitleEnter(UserTg user, string msg)
    {
        var room = new PartyRoom()
        {
            Title = msg,
            Admin = user,
            Users = []
        };

        _db.Rooms.Insert(room);

        user.CurrentState = NameParser.JoinArgs(TITLE, room.Id);
        _db.Users.Update(user);

        return _bot.SendMessage(user.Id,
            _msgDict[_lang].RoomCreationEnterDescription,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task OnDescriptionEnter(UserTg user, string text, string roomId)
    {
        var room = _db.Rooms.FindById(new Guid(roomId));
        room.PartyDescription = text;
        _db.Rooms.Update(room);

        user.CurrentState = NameParser.JoinArgs(RegistrationState.TITLE, roomId);
        _db.Users.Update(user);

        var message = $"{_msgDict[_lang].RoomCreated} {roomId}\n{NameParser.GetRoomJoinLink(roomId)}";

        await _bot.SendMessage(user.Id,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
        await _bot.SendMessage(user.Id,
            _msgDict[_lang].EnterRealName,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }
}
