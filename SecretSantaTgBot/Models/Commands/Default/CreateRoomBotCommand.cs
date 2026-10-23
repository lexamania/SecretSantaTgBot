using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class CreateRoomBotCommand(RoomCreateState createRoomState)
    : BotCommandBase(BotCommandsList.Back)
{
    public override Task Execute(Chat chat, BotCommandContext context)
        => createRoomState.PrepareState(context);
}
