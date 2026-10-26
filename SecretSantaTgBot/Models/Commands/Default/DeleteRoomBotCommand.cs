using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class DeleteRoomBotCommand(
    ServicesContext services,
    RoomDeletingState deleteRoomState
    ) : BotCommandBase(BotCommandsList.DeleteRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.GetAdminRooms().Count() > 0
            ? deleteRoomState.PrepareState(context)
            : services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.ZeroRooms);
    }
}
