using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class ShowRoomsBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRooms)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        var user = context.User;

        if (user.AvailableRooms is { Count: > 0 })
        {
            var message = services.MessageBuilder.BuildRoomsInfoMessage(user);
            return services.Notifications.SendMessage(chat.Id, message);
        }

        return services.Notifications.SendErrorMessage(chat.Id, context.Localization.ZeroRooms);
    }
}
