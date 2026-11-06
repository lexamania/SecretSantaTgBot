using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom.Admin;

public class UpdateRoomCommand(
    ServicesContext services,
    ModifyRoomState roomUpdateState)
    : BotCommandBase(
        BotCommandsList.ModifyRoom,
        access: AccessRights.Admin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.IsAdmin()
            ? roomUpdateState.StartState(chat, context)
            : services.Notifications.SendNeedAdminRights(chat.Id);
    }
}
