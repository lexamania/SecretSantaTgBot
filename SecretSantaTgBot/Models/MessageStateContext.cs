using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Services.Brokers;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

[RequiredService(InjectionType.Singleton)]
public record MessageStateContext(
    SantaDatabase Database,
    NotificationService Notifications,
    LocalizationDispatcher LocalizationDispatcher,
    MessageBuilder MessageBuilder)
{
    public MessageBroker? MessageBroker { get; set; }
};
