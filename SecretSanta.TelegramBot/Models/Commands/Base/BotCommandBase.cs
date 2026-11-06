using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Base;

public abstract class BotCommandBase(
    string command,
    AccessRights? access = AccessRights.Default)
    : BotCommand(command, command), IBotCommand
{
    public AccessRights Access { get; } = access!.Value;
    public abstract Task Execute(Chat chat, MessageContext context);
}
