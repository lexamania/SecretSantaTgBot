using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;
using SecretSanta.Shared.Utils;

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
            ButtonUtil.GetRoomButtons
        );
    }

    protected override IMessageState FirstState => States[StatesList.EnterRoomAlias];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var roomAlias = context.State.Arguments[StateArgsList.RoomAlias];
        var roomId = ButtonUtil.ParseRoomIdFromAlias(roomAlias);
        var room = roomId is { } id
            ? context.User.GetAvailableRoom(id)
            : null;

        if (room is null)
        {
            await Services.Notifications.SendStagedMessage(
                chat.Id, Services.Localizer.Messages.ErrRoomDoesntExist);
            await SendWelcomeMessage(chat, context);
        }
        else
        {
            await Services.Database.Users.EnterRoomAsync(context.User, room);
            await Services.GlobalHandler.ResetStateAsync(chat, context.User.TelegramUserId);
        }

        return true;
    }
}
