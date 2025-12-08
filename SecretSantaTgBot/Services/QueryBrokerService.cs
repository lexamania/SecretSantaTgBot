using SecretSantaTgBot.Storage;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

public class QueryBrokerService
{
    public TelegramBotClient Bot { get; }
    public SantaDatabase DB { get; }

    public QueryBrokerService(TelegramBotClient bot, SantaDatabase db)
    {
        Bot = bot;
        DB = db;
    }

    public async Task OnUpdate(Update update)
    {

    }
}
