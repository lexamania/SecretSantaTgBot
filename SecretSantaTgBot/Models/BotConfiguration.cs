using Microsoft.Extensions.Configuration;

namespace SecretSantaTgBot.Models;

public class BotConfiguration(IConfiguration config) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    public string DBPath { get; } = Path.Join(AppContext.BaseDirectory, "Data", "Storage.db");
    public string LogsDirectoryPath { get; } = Path.Join(AppContext.BaseDirectory, @"Logs");

    public string BotToken { get; } = config.GetValue<string>("BOT_TOKEN")!;
    public string? BotName { get; set; }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
