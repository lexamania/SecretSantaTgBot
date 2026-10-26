using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomSelectionState : InternalMessageStateBase
{
    public RoomSelectionState(ServicesContext services)
        : base(services, StatesList.RoomSelection)
    {
        States[StatesList.RoomRegistration] = new InRoomRegistrationState(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        var buttons = context.User.AvailableRooms
            .Select(MessageParser.GenerateRoomButton)
            .ToArray();
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.ChooseRoom,
            buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleStates(this, chat, context))
            return true;

        if (context.Message.Type == Messages.MessageType.Command)
            return false;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.ChooseRoom);
            await PrepareState(context);
            return true;
        }

        var roomId = MessageParser
            .ParseButtonMessage(context.Message.TextMsg!.Text)
            .Last();
        var room = context.User.GetAvailableRoom(roomId);

        if (room is null)
        {
            await Services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.RoomDoesntExist);
            await PrepareState(context);
            return true;
        }

        Services.Database.Users.UpdateWithRoomSelection(context.User, room);

        var participant = context.User.GetAsParticipant(room);
        if (participant?.RealName is null)
        {
            await States[StatesList.RoomRegistration].PrepareState(context);
            return true;
        }

        await context.GlobalMessageHandler.SendHelpMenu(context.User);
        return true;
    }
}
