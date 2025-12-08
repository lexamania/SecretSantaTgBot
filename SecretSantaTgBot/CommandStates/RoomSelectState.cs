
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RoomSelectState : CommandStateBase
{
    public const string TITLE = "room_selection";

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public RoomSelectState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;
        _csm = csm;
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        var text = msg.Text!.Trim();

        if (text.Length == 0 || text.StartsWith('/'))
            return OnCommandError(msg.Chat);

        var roomId = NameParser.ParseButton(text).Last();
        var room = user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
            return _bot.SendMessage(msg.Chat, _msgDict[_lang].RoomDoesntExist);
        
        user.SelectedRoom = room;
        user.CurrentState = default;
        _db.Users.Update(user);

        return _csm.UpdateAfterStatusChanged(user);
    }

    private Task OnCommandError(Chat chat)
        => _bot.SendMessage(chat, $"{_msgDict[_lang].CommandError}\n\n{_msgDict[_lang].ChooseRoom}");
}
