using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.Shared.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.Base;

public abstract class MessageStateBase(
    string title,
    IMessageState? parrent,
    ServicesContext services)
    : IMessageState
{
    protected ServicesContext Services { get; } = services;
    protected Dictionary<string, IBotCommand> Commands { get; } = [];
    protected Dictionary<string, IMessageState> States { get; } = [];

    public string Title { get; } = title;
    public IMessageState? Parrent { get; } = parrent;
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands => Commands;
    public IReadOnlyDictionary<string, IMessageState> ReadOnlyStates => States;

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
