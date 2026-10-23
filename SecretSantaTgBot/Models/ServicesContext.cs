using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

[RequiredService(InjectionType.Singleton)]
public record ServicesContext(
    SantaDatabase Database,
    NotificationService Notifications,
    MessageBuilder MessageBuilder,
    CustomProcessHandler ProcessHandler);
