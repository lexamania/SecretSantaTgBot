using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands;

public delegate Task BotCommandCallback(Chat chat, BotCommandContext context);
public class CustomBotCommand(
    string command,
    BotCommandCallback callback
    ) : BotCommand(command, command), IBotCommand
{
    public bool HasHelp { get; init; } = true;
    public Base.AccessRights Access { get; init; } = Base.AccessRights.Default;

    public Task Execute(Chat chat, BotCommandContext context)
        => callback.Invoke(chat, context);
}
