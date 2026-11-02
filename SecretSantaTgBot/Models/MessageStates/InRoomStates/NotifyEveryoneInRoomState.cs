using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class NotifyEveryoneInRoomState : MessageStateBase
{
    public NotifyEveryoneInRoomState(ServicesContext services)
        : base(services, StatesList.NotifyEveryone)
    {
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        return Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.EnterMessage,
            MessageParser.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendWrongText(chat.Id);
            return true;
        }

        FinishState(context.User);
        await Task.WhenAll(
            Services.Notifications.NotifyEveryoneInRoom(
                context.User.SelectedRoom!,
                context.Message.TextMsg!.Text),
            Services.Notifications.SendMessageByKey(
                context.User.TelegramUserId,
                BotContent.Messages.MessageIsSent)
        );

        return true;
    }
}
