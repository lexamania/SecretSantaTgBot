using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class MessageStateBase(string[] states)
{
    /// <returns>true - was processed, false - need to process</returns>
    public abstract Task<bool> ProcessMessage(Chat chat, BotCommandContext context);
    public virtual Task PrepareState(BotCommandContext context) => Task.CompletedTask;

    protected Dictionary<string, IBotCommand> Commands { get; } = [];
    protected Dictionary<string, MessageStateBase> States { get; } = [];

    public string StateUrl { get; } = StateUtil.JoinStates(states);
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands => Commands;
    public IReadOnlyDictionary<string, MessageStateBase> ReadOnlyStates => States;
}
