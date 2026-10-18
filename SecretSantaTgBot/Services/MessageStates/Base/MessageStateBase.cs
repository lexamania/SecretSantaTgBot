using SecretSantaTgBot.Localization.Dictionaries;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.Brokers;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class MessageStateBase
{
    public MessageStateBase(MessageStateContext context, string title, bool setHelp = true)
    {
        this.Database = context.Database;
        this.Notifications = context.Notifications;
        this.Localization = context.LocalizationDispatcher.Get("UA");
        this.MessageBuilder = context.MessageBuilder;
        this.MessageBroker = context.MessageBroker!;

        this.Title = title;

        if (setHelp)
            Commands.Add("/help", new("/help", Localization.CommandHelp, CommandHelp));
    }

    public abstract Task<bool> ProcessMessage(Message msg, UserEntity user);
    public virtual Task PrepareState(UserEntity user, string[] args) => Task.CompletedTask;

    protected string Title { get; init; }
    protected Dictionary<string, CommandModel> Commands { get; } = [];

    protected SantaDatabase Database { get; init; }
    protected NotificationService Notifications { get; init; }
    protected ILocalization Localization { get; init; }
    protected MessageBuilder MessageBuilder { get; init; }
    protected MessageBroker MessageBroker { get; init; }

    protected void UpdateUserState(UserEntity user, string state)
        => Database.UserDirectory.UpdateWithState(user, state);

    protected void ClearUserState(UserEntity user)
        => Database.UserDirectory.UpdateWithState(user, default);

    protected Task CommandHelp(Chat chat, UserEntity user, string[] args)
    {
        var isAdmin = user.SelectedRoom?.Admin.Id == user.Id;
        var msg = MessageBuilder.BuildHelpMessage(Commands.Values, isAdmin);
        return Notifications.SendMessage(chat.Id, msg);
    }
}
