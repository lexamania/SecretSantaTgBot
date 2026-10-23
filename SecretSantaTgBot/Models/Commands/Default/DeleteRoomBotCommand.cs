using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class DeleteRoomBotCommand(
    ServicesContext services,
    RoomDeleteState deleteRoomState)
    : BotCommandBase(BotCommandsList.DeleteRoom)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        var user = context.User;
        var rooms = user.AvailableRooms.Where(x => x.Admin.Id == user.Id).ToList();

        return rooms is { Count: > 0 }
            ? deleteRoomState.PrepareState(context)
            : services.Notifications.SendErrorMessage(chat.Id, context.Localization.ZeroRooms);
    }
}
