using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.Base;

public enum AccessRights
{
    Default,
    Admin,
    NotForAdmin
}

public interface IBotCommand
{
    public abstract Task Execute(Chat chat, MessageContext context);

    public string Command { get; }
    public string Description { get; }
    public bool HasHelp { get; }
    public AccessRights Access { get; }
}
