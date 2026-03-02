using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class MessageStateBase
{
    protected string Title { get; }
    protected Dictionary<string, CommandInfo> Commands { get; } = [];

    protected MessageBrokerService Csm { get; }
    protected SantaDatabase DB => Csm.DB;
    protected NotificationService NotifyService => Csm.NotifyService;
    protected static MessagesBase Msgs => EnvVariables.Messages;

    public MessageStateBase(MessageBrokerService csm, string title, bool setHelp = true)
    {
        Csm = csm;
        Title = title;

        if (setHelp)
            Commands.Add("/help", new("/help", Msgs.CommandHelp, CommandHelp));
    }

    public abstract Task<bool> OnMessage(Message msg, UserEntity user);
    public virtual Task StartState(UserEntity user, string[] args) => Task.CompletedTask;

    protected void UpdateUserState(UserEntity user, string? state)
        => DB.UserDirectory.UpdateWithState(user, state);

    protected Task CommandHelp(Chat chat, UserEntity user, string[] args)
    {
        var isAdmin = user.SelectedRoom?.Admin.Id == user.Id;
        var msg = MessageBuilder.BuildHelpMessage(Commands.Values, isAdmin);
        return NotifyService.SendMessage(chat.Id, msg);
    }
}
