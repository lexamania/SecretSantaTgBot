using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class MessageStateBase(ServicesContext services, string title)
{
    protected ServicesContext Services { get; } = services;
    protected Dictionary<string, IBotCommand> Commands { get; } = [];
    protected Dictionary<string, InternalMessageStateBase> States { get; } = [];

    public string Title { get; } = title;
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands => Commands;
    public IReadOnlyDictionary<string, InternalMessageStateBase> ReadOnlyStates => States;

    /// <returns>true - was processed, false - need to process</returns>
    public abstract Task<bool> ProcessMessage(Chat chat, MessageContext context);
}
