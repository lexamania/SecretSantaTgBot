namespace SecretSanta.Localization.Services;

public static class LanguageManager
{
    public const string Default = "uk";

    public static IReadOnlyCollection<string> Supported = [
        "uk"
    ];

    public static string GetSupportedLanguage(string? languageCode)
    {
        var language = ConvertLanguageToSystem(languageCode);
        return language is not null && Supported.Contains(language)
            ? language
            : Default;
    }

    public static string? ConvertLanguageToSystem(string? tgLanguageCode)
    {
        if (string.IsNullOrWhiteSpace(tgLanguageCode))
        {
            return null;
        }

        var language = tgLanguageCode
            .Split('-', 2, StringSplitOptions.TrimEntries)[0]
            .ToLowerInvariant();

        return string.IsNullOrWhiteSpace(language) ? null : language;
    }
}
