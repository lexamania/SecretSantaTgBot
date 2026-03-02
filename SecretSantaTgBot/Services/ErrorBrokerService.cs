using SecretSantaTgBot.Models;

using Telegram.Bot.Polling;

namespace SecretSantaTgBot.Services;

public class ErrorBrokerService(ServiceContainer container)
{
    public Task OnError(Exception exception, HandleErrorSource source)
    {
        container.Logger.LogError(exception);
        return Task.CompletedTask;
    }
}
