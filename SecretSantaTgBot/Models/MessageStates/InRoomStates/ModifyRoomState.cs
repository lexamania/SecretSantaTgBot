using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class ModifyRoomState : InternalMessageStateBase
{
    public ModifyRoomState(ServicesContext services)
        : base(services, StatesList.RoomModifying)
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
                context.Localization.RoomCreationEnterDescription);
            await PrepareState(context);
            return true;
        }

        var room = context.User.SelectedRoom!;
        room.PartyDescription = context.Message.TextMsg!.Text;
        Services.Database.Rooms.Update(room);

        await Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.RoomDescriptionUpdated);
        FinishState(context.User);
        return true;
    }
}
