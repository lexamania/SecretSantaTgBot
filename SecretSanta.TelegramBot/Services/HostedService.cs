using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecretSanta.TelegramBot.Configurations;

namespace SecretSanta.TelegramBot.Services;

// public class HostedService(
//     TelegramBotService telegramBot,
//     BotConfiguration config,
//     ILogger<TelegramBotService> logger)
//     : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         logger.LogInformation($"Telegram bot starting at {DateTime.UtcNow} (UTC)");

//         await telegramBot.Initialize();
        
//         var botInfo = await telegramBot.VerifyBotInfo();
//         config.BotName = botInfo.Username;

//         await Task.Delay(Timeout.Infinite, stoppingToken);

//         logger.LogInformation($"Telegram bot stopping at {DateTime.UtcNow} (UTC)");
//     }
// }

public class HostedService(
    TelegramBotService telegramBot,
    BotConfiguration config,
    ILogger<TelegramBotService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Telegram bot starting at {DateTime.UtcNow} (UTC)");

        await telegramBot.Initialize();
        
        var botInfo = await telegramBot.VerifyBotInfo();
        config.BotName = botInfo.Username;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        config.Cancel();
        logger.LogInformation($"Telegram bot stopping at {DateTime.UtcNow} (UTC)");
        return Task.CompletedTask;
    }
}
