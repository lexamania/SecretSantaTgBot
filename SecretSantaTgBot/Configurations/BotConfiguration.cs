using Microsoft.Extensions.Configuration;

using SecretSantaTgBot.Extensions;

namespace SecretSantaTgBot.Configurations;

public class BotConfiguration(IConfiguration config) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    public string BotToken { get; } = 
        config.GetRequiredValue<string>("BOT_TOKEN");
    public string? BotName { get; set; }

    public string DBPath { get; } =
        Path.Join(AppContext.BaseDirectory, "Data", "Storage.db");

    public string DefaultLanguage { get; } =
        config.GetRequiredValue<string>("DEFAULT_LANGUAGE");

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
