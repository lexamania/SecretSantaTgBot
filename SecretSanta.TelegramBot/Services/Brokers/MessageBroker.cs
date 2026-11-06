using Microsoft.Extensions.Logging;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Services.Helpers;
using SecretSanta.TelegramBot.Services.MessageHandlers;
using SecretSanta.Shared.Storage;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSanta.TelegramBot.Services.Brokers;

[RequiredService(InjectionType.Singleton)]
public class MessageBroker(
    SantaDatabase database,
    NotificationService notifications,
    GlobalMessageHandler globalHandler,
    UserLanguageService userLanguageService,
    LocalizationService localizationService,
    ILogger<MessageBroker> logger)
{
    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = msg.From is not null
            ? database.Users.GetOrCreate(msg.From.Id, msg.From.Username, msg.From.LanguageCode)
            : database.Users.GetOrCreate(msg.Chat.Id, msg.Chat.Username);

        var language = userLanguageService.GetLanguage(user);
        using (localizationService.UseCulture(language))
        {
            try
            {
                if (await globalHandler.HandleMessage(msg, user))
                    return;

                logger.LogError(
                    "Message wasn't handled!",
                    msg.Type,
                    msg.Text,
                    user.CurrentState,
                    user.SelectedRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SERVER ERROR",
                    msg.Type,
                    msg.Text,
                    user.CurrentState,
                    user.SelectedRoom);
            }

            await notifications.SendMessageByKey(
                msg.Chat.Id, BotContent.Messages.ErrServer);
            await globalHandler.ExitState(msg.Chat, user);
        }
    }
}
