using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class StartWishesCommand(InRoomWishesState addWishesState)
    : BotCommandBase(BotCommandsList.StartWishes)
{
    public override Task Execute(Chat chat, BotCommandContext context)
        => addWishesState.PrepareState(context);
}
