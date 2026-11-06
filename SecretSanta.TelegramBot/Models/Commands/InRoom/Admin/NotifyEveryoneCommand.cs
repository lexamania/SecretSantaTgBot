using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom.Admin;

public class NotifyEveryoneCommand(
    ServicesContext services,
    NotifyEveryoneInRoomState notifyEveryoneState)
    : BotCommandBase(
        BotCommandsList.NotifyEveryone,
        access: AccessRights.Admin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.IsAdmin()
            ? notifyEveryoneState.StartState(chat, context)
            : services.Notifications.SendNeedAdminRights(chat.Id);
    }
}
