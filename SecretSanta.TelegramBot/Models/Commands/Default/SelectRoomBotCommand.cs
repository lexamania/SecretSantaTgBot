using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Default;

public class SelectRoomBotCommand(
    ServicesContext services,
    RoomSelectionState selectRoomState)
    : BotCommandBase(BotCommandsList.SelectRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        return context.User.AvailableRooms is { Count: > 0 }
            ? selectRoomState.StartState(chat, context)
            : services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrZeroRooms);
    }
}
