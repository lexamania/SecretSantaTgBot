using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.CommandStates;

public class RegistrationState  : CommandStateBase
{
    public const string TITLE = "registration";

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public RegistrationState(MessageBroker csm)
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
        var args = NameParser.ParseArgs(user.CurrentState!);

        if (text.Length == 0 || text.StartsWith('/'))
        {
            await OnCommandError(msg.Chat);
            return;
        }

        if (args.Length < 2)
        {
            user.CurrentState = default;
            _db.Users.Update(user);
            await OnCommandError(msg.Chat);
            return;
        }

        var roomId = new Guid(args[1]);
        var room = _db.Rooms.FindById(roomId);

        if (!user.AvailableRooms.Contains(room))
            user.AvailableRooms.Add(room);

        room.Users.Add(new()
        {
            Id = user.Id,
            Username = user.Username,
            RealName = text
        });

        user.CurrentState = default;
        _db.Users.Update(user);
        _db.Rooms.Update(room);

        await _bot.SendMessage(msg.Chat,
            _msgDict[_lang].UserParticipationEnd,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());

        await _csm.UpdateAfterStatusChanged(user);
    }

    private Task OnCommandError(Chat chat)
        => _bot.SendMessage(chat, $"{_msgDict[_lang].CommandError}\n\n{_msgDict[_lang].EnterRealName}");
}
