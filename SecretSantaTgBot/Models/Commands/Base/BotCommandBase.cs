using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Base;

public abstract class BotCommandBase(
    string command,
    AccessRights? access = AccessRights.Default,
    bool? hasHelp = true)
    : BotCommand(command, command), IBotCommand
{
    public abstract Task Execute(Chat chat, MessageContext context);

    public bool HasHelp { get; } = hasHelp!.Value;
    public AccessRights Access { get; } = access!.Value;
}
