using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class ModifyRoomState : MessageStateBase
{
    public ModifyRoomState(ServicesContext services)
        : base(services, StatesList.RoomModifying)
    {
        var enterDescriptionState = new EnterValueState(
            services,
            StatesList.EnterDescription,
            StateArgsList.Description,
            BotContent.Messages.EnterDescription,
            ModifyRoom,
            context => MessageParser.GetCommandsAsButtons(this)!
        );
        var enterTitleState = new EnterValueState(
            services,
            StatesList.EnterTitle,
            StateArgsList.Title,
            BotContent.Messages.EnterTitle,
            StateUtil.NextStateCallback(enterDescriptionState),
            context => MessageParser.GetCommandsAsButtons(this)!
        );

        States[enterTitleState.Title] = enterTitleState;
        States[enterDescriptionState.Title] = enterDescriptionState;

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterTitle].PrepareState(context);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleState(this, chat, context)
        || await Services.ProcessHandler.HandleCommand(this, chat, context);

    private Task ModifyRoom(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        room.Title = context.State.StateObject.Arguments[StateArgsList.Title];
        room.Description = context.State.StateObject.Arguments[StateArgsList.Description];
        Services.Database.Rooms.Update(room);

        FinishState(context.User);
        return Services.Notifications.SendMessageByKey(
            context.User.TelegramUserId, BotContent.Messages.RoomModifyed);
    }
}
