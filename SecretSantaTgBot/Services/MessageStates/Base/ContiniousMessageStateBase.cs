using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.Base;

public abstract class ContiniousMessageStateBase : MessageStateBase
{
    public ContiniousMessageStateBase(MessageStateContext context, string title) : base(context, title)
    {
        var command = new CommandModel("/stop", Localization.CommandStop, CommandStop);
        Commands.Add(command.Command, command);
    }
    protected abstract string Message { get; }

    public override Task PrepareState(UserEntity user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        
        UpdateUserState(user, state);

        var buttons = Commands.Select(x => x.Key).ToArray();
        return Notifications.SendMessage(user.Id, Message, buttons!);
    }

    private async Task CommandStop(Chat chat, UserEntity user, string[] args)
    {
        ClearUserState(user);
        await MessageBroker.SendHelpMenu(user);
    }

    protected Task CallRequiredCommand(string command, Message msg, UserEntity user, string[]? args)
    {
        var cmd = Commands!.GetValueOrDefault(command);
        return cmd == null 
            ? Notifications.SendErrorCommandMessage(msg.Chat.Id, Message)
            : cmd.Callback.Invoke(msg.Chat, user, args!);
    }
}
