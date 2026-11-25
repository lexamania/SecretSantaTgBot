using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;
using SecretSanta.Shared.Models;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

public class InRoomRegistrationState : SequentialMessageStateBase
{
    public InRoomRegistrationState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.InRoomRegistration, parrent, services)
    {
        States[StatesList.EnterUserName] = EnterValueState.NewUsername(
            this, services, ProcessMessage);
    }

    protected override IMessageState FirstState => States[StatesList.EnterUserName];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var roomId = context.State.Arguments[StateArgsList.RoomId];
        var userName = context.State.Arguments[StateArgsList.Name];

        var room = RoomId.TryParse(roomId, out var parsedRoomId)
            ? await Services.Database.Rooms.GetByIdAsync(parsedRoomId)
            : null;

        if (room is null)
        {
            await Services.Notifications.SendMessage(
                chat.Id, Services.Localizer.Messages.ErrRoomDoesntExist);
            await Services.GlobalHandler.ExitState(chat, context.User);
            return true;
        }

        if (room.SantaPlayed)
        {
            await Services.Notifications.SendMessage(chat.Id,
                Services.MessageBuilder.BuildErrJoiningRestricted());
            return true;
        }

        await Services.Database.Rooms.AddUserToRoomAsync(context.User, room, userName);
        await Services.Database.Users.EnterRoomAsync(context.User, room);

        var participant = context.User.GetAsParticipant(room)!;

        await Services.Notifications.SendMessage(
            chat.Id, Services.Localizer.Messages.UserRegistered);
        await Services.GlobalHandler.ExitState(chat, context.User);

        await Services.Notifications.NotifyEveryoneInRoom(
            context.User, room,
            Services.MessageBuilder.BuildNewParticipant(room, participant));

        return true;
    }
}
