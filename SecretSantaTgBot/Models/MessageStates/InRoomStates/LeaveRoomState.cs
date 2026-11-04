using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class LeaveRoomState : SequentialMessageStateBase
{
    public LeaveRoomState(
        MessageStateBase parrent,
        ServicesContext services)
        : base(StatesList.RoomLeaving, parrent, services)
    {
        States[StatesList.Confirmation] = new ConfirmationState(
            this, services, ProcessMessage);
    }

    protected override MessageStateBase FirstState => States[StatesList.Confirmation];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        Services.Database.RemoveUserFromRoom(context.User, room);

        await Task.WhenAll(
            Services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.UserLeftRoom),
            Services.Notifications.NotifyEveryoneInRoom(
                context.User, room,
                Services.MessageBuilder.BuildLeave(participant))
        );
        return true;
    }
}
