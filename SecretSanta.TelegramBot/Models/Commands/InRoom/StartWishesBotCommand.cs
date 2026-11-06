using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class StartWishesBotCommand(WishAddingState addWishesState)
    : BotCommandBase(BotCommandsList.AddWishes)
{
    public override Task Execute(Chat chat, MessageContext context)
        => addWishesState.StartState(chat, context);
}
