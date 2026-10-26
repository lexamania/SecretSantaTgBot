using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom.Admin;

public class NotifyEveryoneCommand(
    ServicesContext services,
    NotifyEveryoneInRoomState notifyEveryoneState
    ) : BotCommandBase(
            BotCommandsList.NotifyEveryone,
            access: AccessRights.Admin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.IsAdmin()
            ? notifyEveryoneState.PrepareState(context)
            : services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.NeedAdminRights);
    }
}
