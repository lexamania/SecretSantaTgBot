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
    ILogger<MessageBroker> logger)
{
    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = database.Users.GetOrCreate(msg.Chat);

        try
        {
            if (await globalHandler.Handle(msg, user))
            {
                logger.LogInformation(msg.Text);
                return;
            }

            logger.LogWarning(msg.Text);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, msg.Text);
        }

        database.Users.UpdateWithStateClearing(user);
        await notifications.SendErrorCommandMessage(msg.Chat.Id);
        await globalHandler.SendHelpMenu(user);
    }
}
