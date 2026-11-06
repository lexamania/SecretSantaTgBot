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
        var room = context.User.GetAvailableRoom(roomId);

        if (room is { } && context.User.IsAdmin(room))
        {
            Services.Database.DeleteRoom(context.User, room.Id);

            await Task.WhenAll(
                Services.GlobalHandler.ExitState(chat, context.User),
                Services.Notifications.NotifyEveryoneInRoom(
                    context.User, room,
                    Services.MessageBuilder.BuildRoomDeleted(room)));
        }
        else
        {
            await Services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
            await Services.GlobalHandler.ExitState(chat, context.User, false);
        }

        return true;
    }
}
