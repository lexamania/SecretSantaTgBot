using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

[RequiredService]
public abstract class MessageStateBase
{
    private readonly ServiceContainer _container;

    protected string Title { get; }
    protected Dictionary<string, CommandModel> Commands { get; } = [];

    protected MessageBroker MessageBroker => _container.MessageBroker!;
    protected SantaDatabase Database => _container.Database;
    protected NotificationService Notification => _container.Notification;
    protected static IMessagesDictionary Msgs => EnvVariables.Messages;

    public MessageStateBase(ServiceContainer container, string title, bool setHelp = true)
    {
        _container = container;
        Title = title;

        if (setHelp)
            Commands.Add("/help", new("/help", Msgs.CommandHelp, CommandHelp));
    }

    public virtual Task PrepareState(UserEntity user, string[] args) => Task.CompletedTask;
    public abstract Task<bool> ProcessMessage(Message msg, UserEntity user);

    protected void UpdateUserState(UserEntity user, string? state)
        => Database.UserDirectory.UpdateWithState(user, state);

    protected Task CommandHelp(Chat chat, UserEntity user, string[] args)
    {
        var isAdmin = user.SelectedRoom?.Admin.Id == user.Id;
        var msg = MessageBuilder.BuildHelpMessage(Commands.Values, isAdmin);
        return Notification.SendMessage(chat.Id, msg);
    }
}
