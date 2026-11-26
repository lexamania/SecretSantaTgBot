using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.Common;

public class ConfirmationState(
    IMessageState parrent,
    ServicesContext services,
    Func<Chat, MessageContext, Task> callback)
    : MessageStateBase(StatesList.Confirmation, parrent, services)
{
    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
    {
        var buttons = new string[] {
            Services.Localizer.Buttons.Yes,
            Services.Localizer.Buttons.No
        };
        return Services.Notifications.SendStagedMessage(
            context.User.TelegramUserId,
            Services.Localizer.Messages.MessageAskConfirmation,
            buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type == Messages.MessageType.Text)
        {
            if (CompareButton(context.Message.TextMsg!.Text, Services.Localizer.Buttons.Yes))
            {
                await callback.Invoke(chat, context);
                await Services.GlobalHandler.ResetStateAsync(
                    chat,
                    context.User.TelegramUserId,
                    sendWelcomeMessage: false);
                return true;
            }

            if (CompareButton(context.Message.TextMsg!.Text, Services.Localizer.Buttons.No))
            {
                await Services.GlobalHandler.ResetStateAsync(chat, context.User.TelegramUserId);
                return true;
            }
        }

        await Services.Notifications.SendWrongText(context.User.TelegramUserId);
        return true;
    }

    private bool CompareButton(string msg, string button)
        => string.Equals(
            msg,
            button,
            StringComparison.CurrentCultureIgnoreCase);
}
