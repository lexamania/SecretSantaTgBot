using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class LeaveRoomState : MessageStateBase
{
    public LeaveRoomState(ServicesContext services)
        : base(services, StatesList.RoomLeaving)
    {
        States[StatesList.Confirmation] = new ConfirmationState(
            services, ProcessMessage);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.Confirmation].PrepareState(context);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        Services.Database.RemoveUserFromRoom(context.User, room);

        var message = Services.MessageBuilder.BuildLeave(participant);

        await Task.WhenAll(
            Services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.UserLeftRoom),
            Services.Notifications.NotifyEveryoneInRoom(room, message)
        );
        return true;
    }
}
