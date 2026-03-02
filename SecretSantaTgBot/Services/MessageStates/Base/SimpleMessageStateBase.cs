using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class SimpleMessageStateBase : MessageStateBase
{
    protected abstract string Message { get; }

    public SimpleMessageStateBase(ServiceContainer container, string title) : base(container, title)
    {
        var command = new CommandInfo("/stop", Msgs.CommandStop, CommandStop);
        Commands.Add(command.Command, command);
    }

    public override Task PrepareState(UserEntity user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        
        UpdateUserState(user, state);

        var buttons = Commands.Select(x => x.Key).ToArray();
        return Notification.SendMessage(user.Id, Message, buttons!);
    }

    private async Task CommandStop(Chat chat, UserEntity user, string[] args)
    {
        UpdateUserState(user, default);
        await MessageBroker.SendHelpMenu(user);
    }

    protected Task CallRequiredCommand(string command, Message msg, UserEntity user, string[]? args)
    {
        var cmd = Commands!.GetValueOrDefault(command);
        return cmd == null 
            ? Notification.SendErrorCommandMessage(msg.Chat.Id, Message)
            : cmd.Callback.Invoke(msg.Chat, user, args!);
    }
}
