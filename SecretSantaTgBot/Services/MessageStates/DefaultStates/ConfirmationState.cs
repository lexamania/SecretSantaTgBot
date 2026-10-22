using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class ConfirmationState(
    ServicesContext context,
    string parentTitle,
    string title,
    CommandCallback callback)
    : MessageStateBase(context, NameParser.JoinArgs(parentTitle, title))
{
    private readonly CommandCallback _callback = callback;
    private string Message => Localization.AskConfirmation;

    public override Task PrepareState(UserEntity user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(StatePath, strArgs);
        UpdateUserState(user, state);

        var buttons = new string[] { Localization.ButtonYes, Localization.ButtonNo };
        return Notifications.SendMessage(user.Id, Message, buttons!);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
            return false;

        if (message!.Equals(Localization.ButtonYes, StringComparison.CurrentCultureIgnoreCase))
        {
            var args = StateUtil.ParseState(user.CurrentState, StatePath);
            ClearUserState(user);
            await _callback.Invoke(msg.Chat, user, args);
            return true;
        }

        if (message.Equals(Localization.ButtonNo, StringComparison.CurrentCultureIgnoreCase))
        {
            ClearUserState(user);
            await MessageHandler.SendHelpMenu(user);
            return true;
        }

        await Notifications.SendErrorCommandMessage(user.Id, Message);
        return true;
    }
}
