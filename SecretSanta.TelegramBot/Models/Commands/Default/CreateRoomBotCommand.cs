using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Default;

public class CreateRoomBotCommand(RoomCreationState createRoomState)
    : BotCommandBase(BotCommandsList.CreateRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
        => createRoomState.StartState(chat, context);
}
