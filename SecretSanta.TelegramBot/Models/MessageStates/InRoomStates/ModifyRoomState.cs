using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

public class ModifyRoomState : SequentialMessageStateBase
{
    public ModifyRoomState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.RoomModifying, parrent, services)
    {
        var enterDescriptionState = EnterValueState.NewDiscription(
            this, services, ProcessMessage);
        var enterTitleState = EnterValueState.NewTitle(
            this, services, enterDescriptionState.StartState);

        States[enterTitleState.Title] = enterTitleState;
        States[enterDescriptionState.Title] = enterDescriptionState;
    }

    protected override IMessageState FirstState => States[StatesList.EnterTitle];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        room.Title = context.State.Arguments[StateArgsList.Title];
        room.Description = context.State.Arguments[StateArgsList.Description];
        Services.Database.Rooms.Update(room);

        await Services.Notifications.SendMessageByKey(
            context.User.TelegramUserId, BotContent.Messages.RoomModifyed);
        await Services.GlobalHandler.ExitState(chat, context.User);
        return true;
    }
}
