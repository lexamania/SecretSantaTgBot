using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomSelectionState : MessageStateBase
{
    public RoomSelectionState(ServicesContext services)
        : base(services, StatesList.RoomSelection)
    {
        States[StatesList.EnterRoomAlias] = new EnterValueState(
            services,
            StatesList.EnterRoomAlias,
            StateArgsList.RoomAlias,
            context => context.Localization.ChooseRoom,
            SelectRoom,
            context => context.User.AvailableRooms
                        .Select(MessageParser.GenerateRoomButton)
                        .ToArray()!
        );
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterRoomAlias].PrepareState(context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.ProcessHandler.HandleState(this, chat, context);

    private async Task SelectRoom(Chat chat, MessageContext context)
    {
        var roomAlias = context.State.StateObject.Arguments[StateArgsList.RoomAlias];
        var roomId = MessageParser
            .ParseButtonMessage(roomAlias)
            .Last();
        var room = context.User.GetAvailableRoom(roomId);

        if (room is null)
        {
            await PrepareState(context);
            await Services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.RoomDoesntExist);
            return;
        }

        Services.Database.Users.EnterRoom(context.User, room);
        await context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
