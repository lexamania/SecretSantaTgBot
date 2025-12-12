using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class ConfirmationState(
    MessageBrokerService csm,
    string parentTitle,
    string title,
    CommandCallback callback)
    : MessageStateBase(csm, NameParser.JoinArgs(parentTitle, title))
{
    private readonly CommandCallback _callback = callback;
    private string Message => Msgs.AskConfirmation;

    public override Task StartState(UserTg user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        UpdateUserState(user, state);

        var buttons = new string[] { Msgs.ButtonYes, Msgs.ButtonNo };
        return NotifyService.SendMessage(user.Id, Message, buttons!);
    }

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
            return false;

        if (message!.Equals(Msgs.ButtonYes, StringComparison.CurrentCultureIgnoreCase))
        {
            var args = NameParser.ParseStateArgs(user.CurrentState, Title);
            UpdateUserState(user, default);
            await _callback.Invoke(msg.Chat, user, args);
            return true;
        }

        if (message.Equals(Msgs.ButtonNo, StringComparison.CurrentCultureIgnoreCase))
        {
            UpdateUserState(user, default);
            await Csm.UpdateAfterStatusChanged(user);
            return true;
        }

        await NotifyService.SendErrorCommandMessage(user.Id, Message);
        return true;
    }
}
