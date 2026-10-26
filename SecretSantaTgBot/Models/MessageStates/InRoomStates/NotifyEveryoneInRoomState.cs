using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class NotifyEveryoneInRoomState : InternalMessageStateBase
{
    public NotifyEveryoneInRoomState(ServicesContext services)
        : base(services, StatesList.NotifyEveryone)
    {
        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.EnterInRoomMessage,
            MessageParser.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendErrorCommandMessage(
                chat.Id,
                context.Localization.EnterInRoomMessage);
            await PrepareState(context);
            return true;
        }

        await Task.WhenAll(
            Services.Notifications.NotifyEveryoneInRoom(
                context.User.SelectedRoom!,
                context.Message.TextMsg!.Text),
            Services.Notifications.SendMessage(
                context.User.Id,
                context.Localization.InRoomMessageSend)
        );

        FinishState(context.User);
        await context.GlobalMessageHandler.SendHelpMenu(context.User);
        return true;
    }
}
