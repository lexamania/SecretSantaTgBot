using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Common;

public class ConfirmationState(
    ServicesContext services,
    string title,
    IBotCommand command
    ) : InternalMessageStateBase(services, title)
{
    protected override Task PrepareStateInternal(MessageContext context)
    {
        var buttons = new string[] {
            context.Localization.ButtonYes,
            context.Localization.ButtonNo
        };
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.AskConfirmation,
            buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type != Messages.MessageType.Text)
            return false;

        if (CompareButton(context.Message.TextMsg!.Text, context.Localization.ButtonYes))
        {
            await command.Execute(chat, context);
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
            context.User.Id,
            context.Localization.AskConfirmation);

        return true;
    }

    private bool CompareButton(string msg, string button)
        => msg.Equals(button, StringComparison.CurrentCultureIgnoreCase);
}
