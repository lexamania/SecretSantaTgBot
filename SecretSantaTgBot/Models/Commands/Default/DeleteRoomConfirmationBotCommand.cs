using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class DeleteRoomConfirmationBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.DeleteRoom)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var roomId = context.State.StateObject.Arguments[StateArgsList.RoomId];
        var room = services.Database.DeleteRoom(context.User, roomId);

        var notifyMessage = services.MessageBuilder.BuildDeleteRoomMessage(room);
        await services.Notifications.NotifyEveryoneInRoom(room, notifyMessage);
        await context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
