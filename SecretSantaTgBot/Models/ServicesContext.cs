using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Services.Helpers;
using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

[RequiredService(InjectionType.Singleton)]
public record ServicesContext(
    SantaDatabase Database,
    MessageBuilder MessageBuilder,
    NotificationService Notifications,
    CustomProcessHandler ProcessHandler,
    GlobalMessageHandler GlobalMessageHandler);
