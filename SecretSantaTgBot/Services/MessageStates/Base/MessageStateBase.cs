using SecretSantaTgBot.Localization.Dictionaries;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class MessageStateBase
{
    public MessageStateBase(ServicesContext context, string statePath, bool setHelp = true)
    {
        this.Database = context.Database;
        this.Notifications = context.Notifications;
        this.Localization = context.LocalizationDispatcher.Get("UA");
        this.MessageBuilder = context.MessageBuilder;

        this.StatePath = statePath;

    }

    public abstract Task<bool> ProcessMessage(Message msg, UserEntity user);
    public virtual Task PrepareState(UserEntity user, string[] args) => Task.CompletedTask;

    protected string StatePath { get; init; }
    protected Dictionary<string, IBotCommand> Commands { get; } = [];

    protected SantaDatabase Database { get; init; }
    protected NotificationService Notifications { get; init; }
    protected ILocalization Localization { get; init; }
    protected MessageBuilder MessageBuilder { get; init; }

    protected void UpdateUserState(UserEntity user, string state)
        => Database.UserDirectory.UpdateWithState(user, state);

    protected void ClearUserState(UserEntity user)
        => Database.UserDirectory.UpdateWithClearState(user);
}
