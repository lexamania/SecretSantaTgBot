using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Services.Helpers;
using SecretSanta.TelegramBot.Services.MessageHandlers;
using SecretSanta.Shared.Storage;

namespace SecretSanta.TelegramBot.Models;

[RequiredService(InjectionType.Singleton)]
public record ServicesContext(
    ISantaDatabase Database,
    BotLocalizer Localizer,
    MessageBuilder MessageBuilder,
    NotificationService Notifications,
    GlobalMessageHandler GlobalHandler,
    CustomHandler CustomHandler);
