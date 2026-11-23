using Microsoft.Extensions.Options;

using SecretSanta.Shared.Configurations;

namespace SecretSanta.TelegramBot.Configurations;

public class BotConfiguration(IOptions<TelegramBotOptions> options) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    public string BotToken { get; } = options.Value.Token;
    public string DBPath { get; } = options.Value.DatabasePath;

    public string? BotName { get; set; }

    public void Cancel()
        => _cts.Cancel();

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
