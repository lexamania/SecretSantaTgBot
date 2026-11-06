using Microsoft.Extensions.Configuration;

using SecretSanta.TelegramBot.Extensions;

namespace SecretSanta.TelegramBot.Configurations;

public class BotConfiguration(IConfiguration config) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    public string BotToken { get; } = 
        config.GetRequiredValue<string>("BOT_TOKEN");

    public string? BotName { get; set; }

    public string DBPath { get; } =
        Path.Join(AppContext.BaseDirectory, "Data", "Storage.db");

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
