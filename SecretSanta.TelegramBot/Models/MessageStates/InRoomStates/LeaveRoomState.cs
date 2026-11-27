using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

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

        await Services.Database.Rooms.RemoveUserFromRoomAsync(
            context.User.TelegramUserId,
            room.Id);

        await Services.Notifications.SendMessage(
                chat.Id, Services.Localizer.Messages.UserLeftRoom);
        await Services.GlobalHandler.ResetStateAsync(chat, context.User.TelegramUserId);
        await Services.Notifications.NotifyEveryoneInRoom(
                context.User.TelegramUserId, room,
                Services.MessageBuilder.BuildUserLeft(participant));

        return true;
    }
}
