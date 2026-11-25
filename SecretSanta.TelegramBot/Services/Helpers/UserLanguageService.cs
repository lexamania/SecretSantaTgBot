using SecretSanta.TelegramBot.Attributes;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Localization.Enums;

namespace SecretSanta.TelegramBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public class UserLanguageService
{
    public string GetLanguage(UserEntity user)
        => user.PrefferedLanguage is { } lang
        && LanguageList.Supported.Contains(lang)
            ? lang
            : LanguageList.Default;
}
