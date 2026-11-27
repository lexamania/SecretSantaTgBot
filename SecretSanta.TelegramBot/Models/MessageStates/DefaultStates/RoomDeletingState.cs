using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

public class RoomDeletingState : SequentialMessageStateBase
{
    public RoomDeletingState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.RoomDeleting, parrent, services)
    {
        var confirmDeleteState = new ConfirmationState(
            this, services, ProcessMessage);
        var selectRoomState = EnterValueState.NewRoomAlias(
            this, services,
            confirmDeleteState.StartState,
            async context => ButtonUtil.GetRoomButtons(
                await services.Database.Rooms.GetByAdminAsync(
                    context.User.TelegramUserId))
        );

        States[selectRoomState.Title] = selectRoomState;
        States[confirmDeleteState.Title] = confirmDeleteState;
    }

    protected override IMessageState FirstState => States[StatesList.EnterRoomAlias];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var user = context.User;
        var roomId = ButtonUtil.ParseRoomIdFromAlias(
            context.State.Arguments[StateArgsList.RoomAlias]);
        var room = roomId is { }
            ? await Services.Database.Rooms.GetByIdAsync(roomId.Value)
            : null;

        if (room is { } && user.IsAdmin(room))
        {
            await Services.Database.Rooms.DeleteAsync(room.Id);

            await Task.WhenAll(
                Services.GlobalHandler.ResetStateAsync(chat, user.TelegramUserId),
                Services.Notifications.NotifyEveryoneInRoom(
                    user.TelegramUserId, room,
                    Services.MessageBuilder.BuildRoomDeleted(room)));
        }
        else
        {
            await Services.Notifications.SendStagedMessage(
                chat.Id, Services.Localizer.Messages.ErrRoomDoesntExist);
            await Services.GlobalHandler.ResetStateAsync(
                chat,
                user.TelegramUserId,
                sendWelcomeMessage: false);
        }

        return true;
    }
}
