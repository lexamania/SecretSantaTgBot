using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class NotifyEveryoneInRoomState : MessageStateBase
{
    public NotifyEveryoneInRoomState(
        MessageStateBase parrent,
        ServicesContext services)
        : base(StatesList.NotifyEveryone, parrent, services)
    {
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
    {
        return Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.EnterMessage,
            ButtonUtil.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.CustomHandler.HandleCommand(this, chat, context))
            return true;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendWrongText(chat.Id);
            return true;
        }

        await Services.Notifications.NotifyEveryoneInRoom(
            context.User,
            context.User.SelectedRoom!,
            context.Message.TextMsg!.Text);
        await Services.Notifications.SendMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.MessageIsSent);

        await Services.GlobalHandler.ExitState(chat, context.User, false);
        return true;
    }
}
