using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

[RequiredService(InjectionType.Singleton)]
public record MessageStateContext(
    SantaDatabase Database,
    NotificationService Notifications,
    LocalizationDispatcher LocalizationDispatcher,
    MessageBuilder MessageBuilder)
{
    public GlobalMessageHandler? GlobalMessageHandler { get; set; }
};
