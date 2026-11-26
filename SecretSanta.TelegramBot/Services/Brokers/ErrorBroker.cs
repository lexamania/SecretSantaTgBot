using Microsoft.Extensions.Logging;

using SecretSanta.TelegramBot.Attributes;

using Telegram.Bot.Polling;

namespace SecretSanta.TelegramBot.Services.Brokers;

[RequiredService(InjectionType.Transient)]
public class ErrorBroker(ILogger<ErrorBroker> logger)
{
    public Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Telegram update handler failed. Error source: {ErrorSource}.", source);
        return Task.CompletedTask;
    }
}
