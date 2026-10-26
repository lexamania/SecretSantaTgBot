using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class StartWishesBotCommand(WishAddingState addWishesState)
    : BotCommandBase(BotCommandsList.StartWishes)
{
    public override Task Execute(Chat chat, MessageContext context)
        => addWishesState.PrepareState(context);
}
