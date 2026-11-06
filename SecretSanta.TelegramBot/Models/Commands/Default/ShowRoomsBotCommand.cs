using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Default;

public class ShowRoomsBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRooms)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var user = context.User;

        if (user.AvailableRooms is { Count: > 0 })
        {
            var message = services.MessageBuilder.BuildRoomsList(user);
            return services.Notifications.SendStagedMessage(chat.Id, message);
        }

        return services.Notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.ErrZeroRooms);
    }
}
