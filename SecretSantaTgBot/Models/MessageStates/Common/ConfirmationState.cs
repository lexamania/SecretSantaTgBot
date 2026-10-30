using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Common;

public class ConfirmationState(
    ServicesContext services,
    Func<Chat, MessageContext, Task> callback)
    : MessageStateBase(services, StatesList.Confirmation)
{
    protected override Task PrepareStateInternal(MessageContext context)
    {
        var buttons = new string[] {
            context.Localization.ButtonYes,
            context.Localization.ButtonNo
        };
        return Services.Notifications.SendMessage(
            context.User.TelegramUserId,
            context.Localization.AskConfirmation,
            buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type != Messages.MessageType.Text)
            return false;

        if (CompareButton(context.Message.TextMsg!.Text, context.Localization.ButtonYes))
        {
            await callback.Invoke(chat, context);
            FinishState(context.User);
            return true;
        }

        if (CompareButton(context.Message.TextMsg!.Text, context.Localization.ButtonNo))
        {
            FinishState(context.User);
            await context.GlobalMessageHandler.SendHelpMenu(context.User);
            return true;
        }

        await Services.Notifications.SendErrorCommandMessage(
            context.User.TelegramUserId,
            context.Localization.AskConfirmation);

        return true;
    }

    private bool CompareButton(string msg, string button)
        => msg.Equals(button, StringComparison.CurrentCultureIgnoreCase);
}
