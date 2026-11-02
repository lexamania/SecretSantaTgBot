using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Services.Helpers;
using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Services.Brokers;

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
                if (await globalHandler.Handle(msg, user))
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

            database.Users.ClearState(user);

            await notifications.SendMessageByKey(
                msg.Chat.Id, BotContent.Messages.ErrServer);
            await globalHandler.SendHelpMenu(user);
        }
    }
}
