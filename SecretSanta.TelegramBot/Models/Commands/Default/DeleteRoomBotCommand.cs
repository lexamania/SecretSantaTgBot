using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Default;

public class DeleteRoomBotCommand(
    ServicesContext services,
    RoomDeletingState deleteRoomState)
    : BotCommandBase(BotCommandsList.DeleteRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.GetAdminRooms().Count() > 0
            ? deleteRoomState.StartState(chat, context)
            : services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrZeroRooms);
    }
}
