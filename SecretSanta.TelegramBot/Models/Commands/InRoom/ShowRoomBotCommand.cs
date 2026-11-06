using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class ShowRoomBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRoomInfo)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var message = services.MessageBuilder
            .BuildRoomInfo(context.User.SelectedRoom!);
        return services.Notifications.SendMessage(chat.Id, message);
    }
}
