using SecretSantaTgBot.Storage.Models;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models;

public delegate Task CommandCallback(Chat chat, UserTg user, string[] args);

public class CommandInfo(
    string name, 
    string description, 
    CommandCallback callback)
    : BotCommand(name, description)
{
	public CommandCallback Callback { get; } = callback;
    public bool ShowHelp { get; init; } = true;
    public bool IsForAdmin { get; init; } = false;
}
