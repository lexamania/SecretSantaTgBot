using Microsoft.Extensions.Logging;

using SecretSanta.Localization.Services;
using SecretSanta.TelegramBot.Attributes;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSanta.TelegramBot.Services.Brokers;

[RequiredService(InjectionType.Singleton)]
public class MessageBroker(
    ServicesContext services,
    CultureSwitcher cultureSwitcher,
    ILogger<MessageBroker> logger)
{
    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = msg.From is not null
            ? await services.Database.Users.GetOrCreateAsync(
                msg.From.Id,
                msg.From.Username,
                msg.From.LanguageCode)
            : await services.Database.Users.GetOrCreateAsync(msg.Chat.Id, msg.Chat.Username);

        var language = LanguageManager.GetSupportedLanguage(user.PrefferedLanguage);

        using (cultureSwitcher.UseCulture(language))
        {
            try
            {
                if (await services.GlobalHandler.HandleMessage(msg, user))
                    return;

                logger.LogError(
                    "Message was not handled for chat {ChatId}. " +
                    "Update type: {UpdateType}, message type: {MessageType}, " +
                    "text: {MessageText}, state: {CurrentState}, " +
                    "selected room: {SelectedRoom}.",
                    msg.Chat.Id,
                    type,
                    msg.Type,
                    msg.Text,
                    user.CurrentState,
                    user.SelectedRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to process update for chat {ChatId}. " +
                    "Update type: {UpdateType}, message type: {MessageType}, " +
                    "text: {MessageText}, state: {CurrentState}, " +
                    "selected room: {SelectedRoom}.",
                    msg.Chat.Id,
                    type,
                    msg.Type,
                    msg.Text,
                    user.CurrentState,
                    user.SelectedRoom);
            }

            await services.Notifications.SendMessage(
                msg.Chat.Id,
                services.Localizer.Messages.ErrServer);
            await services.GlobalHandler.ResetStateAsync(msg.Chat, user.TelegramUserId);
        }
    }
}
