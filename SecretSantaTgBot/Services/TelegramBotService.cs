using Microsoft.Extensions.DependencyInjection;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Services.Brokers;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

[RequiredService(InjectionType.Singleton)]
public class TelegramBotService (ServiceProvider provider)
{
    private TelegramBotClient? _bot;
    public TelegramBotClient Bot => _bot!;

    public void Initialize()
    {
        var config = provider.GetRequiredService<BotConfiguration>();
        
        _bot = new TelegramBotClient(
            config.BotToken,
            cancellationToken: config.CancellationToken);

        _bot.OnMessage += provider.GetRequiredService<MessageBroker>().OnMessage;
        _bot.OnUpdate += provider.GetRequiredService<QueryBroker>().OnUpdate;
        _bot.OnError += provider.GetRequiredService<ErrorBroker>().OnError;
    }

    public Task<User> VerifyBotInfo() => _bot!.GetMe();
}
