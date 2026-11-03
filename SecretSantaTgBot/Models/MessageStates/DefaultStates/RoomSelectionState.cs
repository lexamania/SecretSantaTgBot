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
            BotContent.Messages.ChooseRoom,
            ProcessMessage,
            context => context.User.AvailableRooms
                        .Select(MessageParser.GenerateRoomButton)
                        .ToArray()!
        );
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

        if (room is null)
        {
            await Services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
            await PrepareState(context);
        }
        else
        {
            Services.Database.Users.EnterRoom(context.User, room);
            await Services.GlobalMessageHandler.SendHelpMenu(context.User);
        }

        return true;
    }
}
