using Microsoft.Extensions.Configuration;

namespace SecretSanta.TelegramBot.Configurations;

public class BotConfiguration(IConfiguration config) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    public string BotToken { get; } = 
        config.GetRequiredValue<string>("BOT_TOKEN");

    public string DBPath { get; } =
        Path.GetFullPath(config.GetRequiredValue<string>("LiteDb:DatabasePath"));

    public string? BotName { get; set; }

    public void Cancel()
        => _cts.Cancel();

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
