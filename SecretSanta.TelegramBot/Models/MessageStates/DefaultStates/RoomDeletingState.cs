using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;
using SecretSanta.Shared.Utils;

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
            ButtonUtil.GetAdminRoomButtons
        );

        States[selectRoomState.Title] = selectRoomState;
        States[confirmDeleteState.Title] = confirmDeleteState;
    }

    protected override IMessageState FirstState => States[StatesList.EnterRoomAlias];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var roomAlias = context.State.Arguments[StateArgsList.RoomAlias];
        var roomId = ButtonUtil.ParseRoomIdFromAlias(roomAlias);
        var room = roomId is { } id
            ? context.User.GetAvailableRoom(id)
            : null;

        if (room is { } && context.User.IsAdmin(room))
        {
            await Services.Database.Rooms.DeleteAsync(context.User.TelegramUserId, room.Id);

            await Task.WhenAll(
                Services.GlobalHandler.ResetStateAsync(chat, context.User.TelegramUserId),
                Services.Notifications.NotifyEveryoneInRoom(
                    context.User, room,
                    Services.MessageBuilder.BuildRoomDeleted(room)));
        }
        else
        {
            await Services.Notifications.SendStagedMessage(
                chat.Id, Services.Localizer.Messages.ErrRoomDoesntExist);
            await Services.GlobalHandler.ResetStateAsync(
                chat,
                context.User.TelegramUserId,
                sendWelcomeMessage: false);
        }

        return true;
    }
}
