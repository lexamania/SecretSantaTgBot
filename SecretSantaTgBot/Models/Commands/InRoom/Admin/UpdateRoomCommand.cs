using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom.Admin;

public class UpdateRoomCommand(
    ServicesContext services,
    InRoomUpdateState roomUpdateState)
    : BotCommandBase(BotCommandsList.UpdateRoom)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        return context.User.IsAdmin()
            ? roomUpdateState.PrepareState(context)
            : services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.NeedAdminRights);
    }
}
