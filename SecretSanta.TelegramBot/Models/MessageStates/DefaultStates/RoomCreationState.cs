using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.DefaultStates;

public class RoomCreationState : SequentialMessageStateBase
{
    public RoomCreationState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.RoomCreation, parrent, services)
    {
        var enterNameState = EnterValueState.NewUsername(
            this, services, ProcessMessage);
        var enterDescriptionState = EnterValueState.NewDiscription(
            this, services, enterNameState.StartState);
        var enterTitleState = EnterValueState.NewTitle(
            this, services, enterDescriptionState.StartState);

        States[enterTitleState.Title] = enterTitleState;
        States[enterDescriptionState.Title] = enterDescriptionState;
        States[enterNameState.Title] = enterNameState;
    }

    protected override IMessageState FirstState => States[StatesList.EnterTitle];

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        var user = context.User;
        var title = context.State.Arguments[StateArgsList.Title];
        var description = context.State.Arguments[StateArgsList.Description];
        var adminName = context.State.Arguments[StateArgsList.Name];

        var room = Services.Database.CreateRoom(user, title, description, adminName);

        var message = Services.MessageBuilder.BuildRoomCreated(room.Id.ToString());
        await Services.Notifications.SendMessage(user.TelegramUserId, message);

        Services.Database.Users.EnterRoom(user, room);
        await Services.GlobalHandler.ExitState(chat, context.User);

        return true;
    }
}
