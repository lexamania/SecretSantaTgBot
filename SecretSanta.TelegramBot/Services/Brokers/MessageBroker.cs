using Microsoft.Extensions.Logging;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Services.Helpers;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSanta.TelegramBot.Services.Brokers;

[RequiredService(InjectionType.Singleton)]
public class MessageBroker(
    ServicesContext services,
    UserLanguageService userLanguageService,
    CultureSwitcher cultureSwitcher,
    ILogger<MessageBroker> logger)
{
    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = msg.From is not null
            ? await services.Database.Users.GetOrCreateAsync(msg.From.Id, msg.From.Username, msg.From.LanguageCode)
            : await services.Database.Users.GetOrCreateAsync(msg.Chat.Id, msg.Chat.Username);

        var language = userLanguageService.GetLanguage(user);
        using (cultureSwitcher.UseCulture(language))
        {
            try
            {
                if (await services.GlobalHandler.HandleMessage(msg, user))
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

            await services.Notifications.SendMessage(msg.Chat.Id, services.Localizer.Messages.ErrServer);
            await services.GlobalHandler.ExitState(msg.Chat, user);
        }
    }
}
