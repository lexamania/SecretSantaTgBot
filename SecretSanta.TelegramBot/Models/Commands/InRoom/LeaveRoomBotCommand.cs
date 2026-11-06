using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class LeaveRoomBotCommand(
    ServicesContext services,
    LeaveRoomState leaveRoomState)
    : BotCommandBase(
        BotCommandsList.LeaveRoom,
        access: AccessRights.NotForAdmin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return !context.User.IsAdmin()
            ? leaveRoomState.StartState(chat, context)
            : services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrAdminCantLeaveRoom);
    }
}
