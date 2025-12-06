using Telegram.Bot.Polling;

namespace SecretSantaTgBot;

public class ErrorBroker(CancellationTokenSource cts)
{
    private readonly CancellationTokenSource _cts = cts;

    public async Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        await Task.Delay(2000, _cts.Token); // delay for 2 seconds before eventually trying again
    }
}
