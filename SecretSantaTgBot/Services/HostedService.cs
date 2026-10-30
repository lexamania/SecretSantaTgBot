using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Configurations;

namespace SecretSantaTgBot.Services;

public class HostedService(
    TelegramBotService telegramBot,
    BotConfiguration config,
    ILogger<TelegramBotService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Telegram bot starting at {DateTime.UtcNow} (UTC)");

        telegramBot.Initialize();
        
        var botInfo = await telegramBot.VerifyBotInfo();
        config.BotName = botInfo.Username;

        await Task.Delay(Timeout.Infinite, stoppingToken);

        logger.LogInformation($"Telegram bot stopping at {DateTime.UtcNow} (UTC)");
    }
}
