using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Services.Brokers;

namespace SecretSantaTgBot.Services;

public class HostedService(
    TelegramBotService telegramBot,
    BotConfiguration config,
    ILogger<TelegramBotService> logger,
    IServiceProvider provider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Telegram bot starting at {DateTime.UtcNow} (UTC)");

        telegramBot.Initialize(
            config.BotToken,
            config.CancellationToken,
            provider.GetRequiredService<MessageBroker>().OnMessage,
            provider.GetRequiredService<QueryBroker>().OnUpdate,
            provider.GetRequiredService<ErrorBrokerService>().OnError);
        
        var botInfo = await telegramBot.VerifyBotInfo();
        config.BotName = botInfo.Username;

        await Task.Delay(Timeout.Infinite, stoppingToken);

        logger.LogInformation($"Telegram bot stopping at {DateTime.UtcNow} (UTC)");
    }
}
