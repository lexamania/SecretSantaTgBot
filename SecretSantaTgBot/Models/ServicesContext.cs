using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

[RequiredService(InjectionType.Singleton)]
public record ServicesContext(
    SantaDatabase Database,
    NotificationService Notifications,
    LocalizationDispatcher LocalizationDispatcher,
    MessageBuilder MessageBuilder);
