using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Attributes;

using Telegram.Bot.Polling;

namespace SecretSantaTgBot.Services;

[RequiredService]
public class ErrorBrokerService(ILogger<ErrorBrokerService> logger)
{
    public Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}
