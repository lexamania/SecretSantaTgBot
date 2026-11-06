using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;
using SecretSanta.TelegramBot.Utils;

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
        var room = context.User.GetAvailableRoom(roomId);

        if (room is null)
        {
            await Services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
            await SendWelcomeMessage(chat, context);
        }
        else
        {
            Services.Database.Users.EnterRoom(context.User, room);
            await Services.GlobalHandler.ExitState(chat, context.User);
        }

        return true;
    }
}
