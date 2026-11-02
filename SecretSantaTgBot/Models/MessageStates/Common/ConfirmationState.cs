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
            Services.MessageBuilder.GetByKey(BotContent.Buttons.Yes),
            Services.MessageBuilder.GetByKey(BotContent.Buttons.No)
        };
        return Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.MessageAskConfirmation,
            buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type == Messages.MessageType.Text)
        {
            if (CompareButton(
                context.Message.TextMsg!.Text,
                Services.MessageBuilder.GetByKey(BotContent.Buttons.Yes)))
            {
                await callback.Invoke(chat, context);
                FinishState(context.User);
                return true;
            }

            if (CompareButton(
                context.Message.TextMsg!.Text,
                Services.MessageBuilder.GetByKey(BotContent.Buttons.No)))
            {
                FinishState(context.User);
                await Services.GlobalMessageHandler.SendHelpMenu(context.User);
                return true;
            }
        }

        await Services.Notifications.SendWrongText(context.User.TelegramUserId);
        return true;
    }

    private bool CompareButton(string msg, string button)
        => msg.Equals(button, StringComparison.CurrentCultureIgnoreCase);
}
