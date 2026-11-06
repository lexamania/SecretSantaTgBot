using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

public class NotifyEveryoneInRoomState : MessageStateBase
{
    public NotifyEveryoneInRoomState(
        IMessageState parrent,
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
