using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands;

public delegate Task BotCommandCallback(Chat chat, MessageContext context);
public class CustomBotCommand(
    string command,
    BotCommandCallback callback
    ) : BotCommand(command, command), IBotCommand
{
    public bool HasHelp { get; init; } = true;
    public AccessRights Access { get; init; } = AccessRights.Default;

    public Task Execute(Chat chat, MessageContext context)
        => callback.Invoke(chat, context);
}
