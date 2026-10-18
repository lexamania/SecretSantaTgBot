using SecretSantaTgBot.Attributes;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

[RequiredService(InjectionType.Singleton)]
public class TelegramBotService
{
    private TelegramBotClient? _bot;

    public TelegramBotClient Bot => _bot!;

    public void Initialize(
        string botToken,
        CancellationToken cancellationToken,
        TelegramBotClient.OnMessageHandler onMessage,
        TelegramBotClient.OnUpdateHandler onUpdate,
        TelegramBotClient.OnErrorHandler onError)
    {
        _bot = new(botToken, cancellationToken: cancellationToken);

        _bot.OnError += onError;
        _bot.OnMessage += onMessage;
        _bot.OnUpdate += onUpdate;
    }

    public Task<User> VerifyBotInfo() => _bot!.GetMe();
}
