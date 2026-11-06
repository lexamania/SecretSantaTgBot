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
            if (CompareButton(context.Message.TextMsg!.Text,BotContent.Buttons.Yes))
            {
                await callback.Invoke(chat, context);
                await Services.GlobalHandler.ExitState(chat, context.User, false);
                return true;
            }

            if (CompareButton(context.Message.TextMsg!.Text, BotContent.Buttons.No))
            {
                await services.GlobalHandler.ExitState(chat, context.User);
                return true;
            }
        }

        await Services.Notifications.SendWrongText(context.User.TelegramUserId);
        return true;
    }

    private bool CompareButton(string msg, string buttonKey)
        => string.Equals(
            msg,
            Services.MessageBuilder.GetByKey(buttonKey),
            StringComparison.CurrentCultureIgnoreCase);
}
