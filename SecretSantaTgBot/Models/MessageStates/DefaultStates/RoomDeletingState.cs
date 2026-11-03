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
            services, ProcessMessage);
        var selectRoomState = new EnterValueState(
            services,
            StatesList.EnterRoomAlias,
            StateArgsList.RoomAlias,
            BotContent.Messages.ChooseRoom,
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

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var roomAlias = context.State.StateObject.Arguments[StateArgsList.RoomAlias];
        var roomId = MessageParser
            .ParseButtonMessage(roomAlias)
            .Last();
        var room = context.User.GetAvailableRoom(roomId);

        FinishState(context.User);

        if (room is { } && context.User.IsAdmin(room))
        {
            var notifyMessage = Services.MessageBuilder.BuildDeleteRoom(room);
            Services.Database.DeleteRoom(context.User, room.Id);

            await Task.WhenAll(
                Services.GlobalMessageHandler.SendHelpMenu(context.User),
                Services.Notifications.NotifyEveryoneInRoom(room, notifyMessage));
        }
        else
        {
            await Services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
        }

        return true;
    }
}
