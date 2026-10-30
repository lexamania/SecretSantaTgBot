using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Default;

public class CreateRoomBotCommand(RoomCreationState createRoomState)
    : BotCommandBase(BotCommandsList.CreateRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
        => createRoomState.PrepareState(context);
}
