using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class SimpleMessageStateBase : MessageStateBase
{
    protected abstract string Message { get; }

    public SimpleMessageStateBase(MessageBrokerService csm, string title) : base(csm, title)
    {
        var command = new CommandInfo("/stop", Msgs.CommandStop, CommandStop);
        Commands.Add(command.Command, command);
    }

    public override Task StartState(UserTg user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        UpdateUserState(user, state);

        var buttons = Commands.Select(x => x.Key).ToArray();
        return NotifyService.SendMessage(user.Id, Message, buttons!);
    }

    private async Task CommandStop(Chat chat, UserTg user, string[] args)
    {
        UpdateUserState(user, default);
        await Csm.UpdateAfterStatusChanged(user);
    }

    protected Task CallRequiredCommand(string command, Message msg, UserTg user, string[]? args)
    {
        var cmd = Commands!.GetValueOrDefault(command);
        return cmd == null 
            ? NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message)
            : cmd.Callback.Invoke(msg.Chat, user, args!);
    }
}
