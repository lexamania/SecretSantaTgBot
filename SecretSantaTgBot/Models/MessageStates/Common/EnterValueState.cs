using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.Models.MessageStates.Common;

public class EnterValueState(
    ServicesContext services,
    string title,
    string valueKey,
    Func<MessageContext, string> enterMessage,
    Func<Chat, MessageContext, Task> postProcess,
    Func<MessageContext, ReplyMarkup>? buttons = default)
    : MessageStateBase(services, title)
{
    protected override Task PrepareStateInternal(MessageContext context)
        => Services.Notifications.SendMessage(
            context.User.TelegramUserId,
            enterMessage.Invoke(context),
            buttons?.Invoke(context));

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type == Messages.MessageType.Command)
            return false;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendErrorCommandMessage(chat.Id);
            await PrepareState(context);
            return true;
        }

        var stateObj = context.State.StateObject;
        stateObj.Arguments[valueKey] = context.Message.TextMsg!.Text;
        stateObj.States.RemoveAt(stateObj.States.Count - 1);

        Services.Database.Users.SetState(context.User, stateObj.ToString());

        await postProcess(chat, context);
        return true;
    }
}
