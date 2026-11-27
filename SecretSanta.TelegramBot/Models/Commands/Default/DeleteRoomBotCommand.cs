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
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var rooms = await services.Database.Rooms.GetByAdminAsync(
            context.User.TelegramUserId);

        if (rooms.Count == 0)
        {
            await services.Notifications.SendMessage(
                chat.Id, services.Localizer.Messages.ErrZeroRooms);
            return;
        }

        await deleteRoomState.StartState(chat, context);
    }
}
