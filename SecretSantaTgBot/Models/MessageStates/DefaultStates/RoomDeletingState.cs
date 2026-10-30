using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomDeletingState : MessageStateBase
{
    public RoomDeletingState(ServicesContext services)
        : base(services, StatesList.RoomDeleting)
    {
        var confirmDeleteState = new ConfirmationState(
            services,
            DeleteRoom);
        var selectRoomState = new EnterValueState(
            services,
            StatesList.EnterRoomAlias,
            StateArgsList.RoomAlias,
            context => context.Localization.ChooseRoom,
            StateUtil.NextStateCallback(confirmDeleteState),
            context => context.User.GetAdminRooms()
                        .Select(MessageParser.GenerateRoomButton)
                        .ToArray()!
        );

        States[selectRoomState.Title] = selectRoomState;
        States[confirmDeleteState.Title] = confirmDeleteState;
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterRoomAlias].PrepareState(context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.ProcessHandler.HandleState(this, chat, context);

    private Task DeleteRoom(Chat chat, MessageContext context)
    {
        var roomAlias = context.State.StateObject.Arguments[StateArgsList.RoomAlias];
        var roomId = MessageParser
            .ParseButtonMessage(roomAlias)
            .Last();
        var room = context.User.GetAvailableRoom(roomId);

        FinishState(context.User);

        if (room is { } && context.User.IsAdmin(room))
        {
            Services.Database.DeleteRoom(context.User, room.Id);

            var notifyMessage = Services.MessageBuilder.BuildDeleteRoomMessage(room);
            return Task.WhenAll(
                context.GlobalMessageHandler.SendHelpMenu(context.User),
                Services.Notifications.NotifyEveryoneInRoom(room, notifyMessage));
        }

        return Services.Notifications.SendErrorMessage(
            chat.Id,
            context.Localization.RoomDoesntExist);

    }
}
