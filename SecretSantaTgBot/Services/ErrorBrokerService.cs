using Telegram.Bot.Polling;

namespace SecretSantaTgBot.Services;

public class ErrorBrokerService(CancellationTokenSource cts)
{
    private readonly CancellationTokenSource _cts = cts;

    public Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
}
