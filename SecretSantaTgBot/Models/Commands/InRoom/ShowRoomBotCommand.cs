using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class ShowRoomBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRoomInfo)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var message = services.MessageBuilder
            .BuildRoomInfoMessage(context.User.SelectedRoom!);
        return services.Notifications.SendMessage(chat.Id, message);
    }
}
