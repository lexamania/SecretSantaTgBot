using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Models;

using Telegram.Bot;

namespace SecretSantaTgBot.Services;

public class TelegramBotService(
    BotConfiguration config,
    ILogger<TelegramBotService> logger,
    IServiceProvider provider)
    : BackgroundService
{
    private TelegramBotClient? _bot;

    public TelegramBotClient Bot => _bot!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Telegram bot starting at {DateTime.UtcNow} (UTC)");

        _bot = new(config.BotToken, cancellationToken: config.CancellationToken);
        
        Initialize();

        await VerifyConnection();

        await Task.Delay(Timeout.Infinite, stoppingToken);

        logger.LogInformation($"Telegram bot stopping at {DateTime.UtcNow} (UTC)");
    }

    private void Initialize()
    {
        var errorBroker = provider.GetRequiredService<ErrorBrokerService>();
        var msgBroker = provider.GetRequiredService<MessageBroker>();
        var queryBroker = provider.GetRequiredService<QueryBroker>();

        _bot!.OnError += errorBroker.OnError;
        _bot.OnMessage += msgBroker.OnMessage;
        _bot.OnUpdate += queryBroker.OnUpdate;
    }

    private async Task VerifyConnection()
    {
        var me = await _bot!.GetMe();
        config.BotName = me.Username!;
    }
}
