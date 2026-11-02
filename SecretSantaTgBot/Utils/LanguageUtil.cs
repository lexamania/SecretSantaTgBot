namespace SecretSantaTgBot.Utils;

public static class LanguageUtil
{
    public static string? ConvertLanguageToSystem(string? tgLanguageCode)
        => tgLanguageCode?.Split('-')[0].ToLowerInvariant();
}
