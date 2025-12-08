using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.CommandStates;

public class InRoomWishesState : CommandStateBase
{
    public const string TITLE = "in_room_wishes";

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public InRoomWishesState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;
        _csm = csm;
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        var room = user.SelectedRoom!;
        var me = room.Users.First(x => x.Id == user.Id);

        if (msg.Type == MessageType.Text)
        {
            var text = msg.Text!.Trim();
            if (text is not { Length: > 0 } || text.StartsWith('/'))
            {
                var error = $"{_msgDict[_lang].CommandError}\n\n{_msgDict[_lang].UserStartWishes}";
                return _bot.SendMessage(msg.Chat, error);
            }

            me.Wishes.Add(new()
            {
                Message = text,
            });
        }
        else if (msg.Type == MessageType.Photo)
        {
            var lastWish = me.Wishes.LastOrDefault();
            if (msg.Caption is null && lastWish is not null && lastWish.Images.Count > 0)
            {
                lastWish.Images.Add(msg.Photo!.First().FileId);
            }
            else
            {
                var wish = new UserWish()
                {
                    Message = msg.Caption,
                    Images = [msg.Photo!.First().FileId]
                };
                me.Wishes.Add(wish);
            }
        }

        _db.Rooms.Update(room);

        return _bot.SendMessage(user.Id,
            _msgDict[_lang].UserWishAdded,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }
}
