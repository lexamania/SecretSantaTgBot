using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class MessageStateBase(
    string title,
    MessageStateBase? parrent,
    ServicesContext services)
{
    protected ServicesContext Services { get; } = services;
    protected Dictionary<string, IBotCommand> Commands { get; } = [];
    protected Dictionary<string, MessageStateBase> States { get; } = [];

    public string Title { get; } = title;
    public MessageStateBase? ParrentState { get; } = parrent;
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands => Commands;
    public IReadOnlyDictionary<string, MessageStateBase> ReadOnlyStates => States;

    public async Task StartState(Chat chat, MessageContext context)
    {
        await PrepareState(context);
        await SendWelcomeMessage(chat, context);
    }

    public virtual Task PrepareState(MessageContext context)
    {
        context.State.States = StateUtil.CalculateStates(this);
        Services.Database.Users.SetState(context.User, context.State);
        return Task.CompletedTask;
    }

    public abstract Task SendWelcomeMessage(Chat chat, MessageContext context);
    public abstract Task<bool> ProcessMessage(Chat chat, MessageContext context);
}
