using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Base;

public abstract class BotCommandBase(string command)
    : BotCommand(command, command), IBotCommand
{
    public abstract Task Execute(Chat chat, BotCommandContext context);

    public bool HasHelp { get; init; } = true;
    public AccessRights Access { get; init; } = AccessRights.Default;
}
