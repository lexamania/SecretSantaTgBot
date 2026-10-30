using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class MessageStateBase(ServicesContext services, string title)
{
    protected ServicesContext Services { get; } = services;
    protected Dictionary<string, IBotCommand> Commands { get; } = [];
    protected Dictionary<string, MessageStateBase> States { get; } = [];

    public string Title { get; } = title;
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands => Commands;
    public IReadOnlyDictionary<string, MessageStateBase> ReadOnlyStates => States;

    /// <returns>true - was processed, false - need to process</returns>
    public abstract Task<bool> ProcessMessage(Chat chat, MessageContext context);

    protected virtual Task PrepareStateInternal(MessageContext context) => Task.CompletedTask;
    public Task PrepareState(MessageContext context)
    {
        var state = context.State.StateObject;
        state.States.Add(Title);
        Services.Database.Users.SetState(context.User, state.ToString());

        return PrepareStateInternal(context);
    }

    protected void FinishState(UserEntity user)
    {
        Services.Database.Users.ClearState(user);
    }
}
