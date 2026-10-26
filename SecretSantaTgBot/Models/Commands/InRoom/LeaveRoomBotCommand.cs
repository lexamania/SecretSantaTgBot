using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class LeaveRoomBotCommand(
    ServicesContext services,
    ConfirmationState confirmationState
    ) : BotCommandBase(
            BotCommandsList.LeaveRoom,
            access: AccessRights.NotForAdmin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return !context.User.IsAdmin()
            ? confirmationState.PrepareState(context)
            : services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.AdminCantLeaveRoom);
    }
}
