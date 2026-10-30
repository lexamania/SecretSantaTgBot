using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class SelectRoomBotCommand(
    ServicesContext services,
    RoomSelectionState selectRoomState)
    : BotCommandBase(BotCommandsList.SelectRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.AvailableRooms is { Count: > 0 }
            ? selectRoomState.PrepareState(context)
            : services.Notifications.SendErrorMessage(chat.Id, context.Localization.ZeroRooms);
    }
}
