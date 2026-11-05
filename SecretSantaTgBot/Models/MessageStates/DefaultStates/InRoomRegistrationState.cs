using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

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

        var room = Services.Database.Rooms.GetById(roomId);
        if (room is null)
        {
            await Services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
            await Services.GlobalHandler.ExitState(chat, context.User);
            return true;
        }

        if (room.SantaPlayed)
        {
            await Services.Notifications.SendMessage(chat.Id,
                Services.MessageBuilder.BuildErrJoiningRestricted());
            return true;
        }

        Services.Database.AddUserToRoom(context.User, room, userName);
        Services.Database.Users.EnterRoom(context.User, room);

        var participant = context.User.GetAsParticipant(room)!;

        await Services.Notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.UserRegistered);
        await Services.GlobalHandler.ExitState(chat, context.User);

        await Services.Notifications.NotifyEveryoneInRoom(
            context.User, room,
            Services.MessageBuilder.BuildNewParticipant(room, participant));

        return true;
    }
}
