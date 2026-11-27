using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

public class RoomSelectionState : SequentialMessageStateBase
{
    public RoomSelectionState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.RoomSelection, parrent, services)
    {
        States[StatesList.EnterRoomAlias] = EnterValueState.NewRoomAlias(
            this, services,
            ProcessMessage,
            async context => ButtonUtil.GetRoomButtons(
                await services.Database.Rooms.GetByParticipantAsync(
                    context.User.TelegramUserId))
        );
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

        if (room is null || !user.IsParticipant(room))
        {
            await Services.Notifications.SendStagedMessage(
                chat.Id, Services.Localizer.Messages.ErrRoomDoesntExist);
            await SendWelcomeMessage(chat, context);
        }
        else
        {
            await Services.Database.Users.EnterRoomAsync(user, room);
            await Services.GlobalHandler.ResetStateAsync(chat, user.TelegramUserId);
        }

        return true;
    }
}
