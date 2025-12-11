using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
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

    public MessageStateBase(MessageBrokerService csm, string title)
    {
        Csm = csm;
        Title = title;
        Commands.Add("/help", new("/help", Msgs.CommandHelp, CommandHelp));
    }

    public abstract Task<bool> OnMessage(Message msg, UserTg user);
    public virtual Task StartState(UserTg user, string[] args) => Task.CompletedTask;

    protected void UpdateUserState(UserTg user, string? state)
    {
        user.CurrentState = state;
        DB.Users.Update(user);
    }

    protected Task CommandHelp(Chat chat, UserTg user, string[] args)
    {
        var msg = MessageBuilder.BuildHelpMessage(Commands.Values, true);
        return NotifyService.SendMessage(chat.Id, msg);
    }
}
