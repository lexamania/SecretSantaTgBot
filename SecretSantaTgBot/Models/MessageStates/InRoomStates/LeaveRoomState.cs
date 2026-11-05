using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class LeaveRoomState : SequentialMessageStateBase
{
    public LeaveRoomState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.RoomLeaving, parrent, services)
    {
        States[StatesList.Confirmation] = new ConfirmationState(
            this, services, ProcessMessage);
    }

    protected override IMessageState FirstState => States[StatesList.Confirmation];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        if (room.SantaPlayed)
        {
            await Services.Notifications.SendMessage(chat.Id,
                Services.MessageBuilder.BuildErrLeavingRestricted());
            return true;
        }

        Services.Database.RemoveUserFromRoom(context.User, room);

        await Services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.UserLeftRoom);
        await Services.GlobalHandler.ExitState(chat, context.User);
        await Services.Notifications.NotifyEveryoneInRoom(
                context.User, room,
                Services.MessageBuilder.BuildUserLeft(participant));

        return true;
    }
}
