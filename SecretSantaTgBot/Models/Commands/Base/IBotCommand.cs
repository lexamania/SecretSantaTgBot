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
    public string Command { get; }
    public string Description { get; }
    public AccessRights Access { get; }

    public abstract Task Execute(Chat chat, MessageContext context);
}
