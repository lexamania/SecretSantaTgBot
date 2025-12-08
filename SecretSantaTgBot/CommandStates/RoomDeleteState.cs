using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.CommandStates;

public class RoomDeleteState : CommandStateBase
{
    public const string TITLE = "room_delete";

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public RoomDeleteState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;
        _csm = csm;
    }

    public override async Task OnMessage(Message msg, UserTg user)
    {
        var text = msg.Text!.Trim();

        if (text.Length == 0 || text.StartsWith('/'))
        {
            await OnCommandError(msg.Chat);
            return;
        }

        var roomId = NameParser.ParseButton(text).Last();
        var room = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
        {
            await _bot.SendMessage(msg.Chat, _msgDict[_lang].RoomDoesntExist);
            return;
        }
        
        user.AvailableRooms.Remove(room);
        user.CurrentState = default;

        _db.Rooms.Delete(room.Id);
        _db.Users.Update(user);

        await _bot.SendMessage(msg.Chat, 
            $"{room.Title} ({room.Id}) {_msgDict[_lang].RoomDeleted}",
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
        await _csm.UpdateAfterStatusChanged(user);
    }

    private Task OnCommandError(Chat chat)
        => _bot.SendMessage(chat, $"{_msgDict[_lang].CommandError}\n\n{_msgDict[_lang].ChooseRoom}");
}
