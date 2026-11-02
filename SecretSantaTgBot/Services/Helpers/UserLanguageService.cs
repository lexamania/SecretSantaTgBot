using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public class UserLanguageService
{
    public readonly string DefaultLanguage = "uk";

    public readonly IReadOnlyCollection<string> SupportedLanguages = [
        "uk"
    ];

    public string GetLanguage(UserEntity user)
        => user.PrefferedLanguage is { } lang && SupportedLanguages.Contains(lang)
            ? lang
            : DefaultLanguage;
}
