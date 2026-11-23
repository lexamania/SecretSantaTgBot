namespace SecretSanta.Shared.Configurations;

public sealed class TelegramBotOptions
{
    public const string SectionName = "TelegramBot";

    public string Token { get; set; } = string.Empty;
    public string DatabasePath { get; set; } = string.Empty;
}
