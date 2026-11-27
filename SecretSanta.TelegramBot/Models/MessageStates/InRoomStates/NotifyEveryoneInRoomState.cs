using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;

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
        return Services.Notifications.SendStagedMessage(
            context.User.TelegramUserId,
            Services.Localizer.Messages.EnterMessage,
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
            context.User.TelegramUserId,
            context.User.SelectedRoom!,
            context.Message.TextMsg!.Text);
        await Services.Notifications.SendMessage(
            context.User.TelegramUserId,
            Services.Localizer.Messages.MessageIsSent);

        await Services.GlobalHandler.ResetStateAsync(
            chat,
            context.User.TelegramUserId,
            sendWelcomeMessage: false);
        return true;
    }
}
