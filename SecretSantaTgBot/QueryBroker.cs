using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot;

public class QueryBroker
{
    public TelegramBotClient Bot { get; }
    public SantaDatabase DB { get; }
    public MessagesDictionary MsgDict { get; }
    public string Lang { get; }

    public QueryBroker(TelegramBotClient bot, SantaDatabase db, MessagesDictionary msgDict)
    {
        Bot = bot;
        DB = db;
        MsgDict = msgDict;
        Lang = "UA";
    }

    public async Task OnUpdate(Update update)
    {

    }
}
