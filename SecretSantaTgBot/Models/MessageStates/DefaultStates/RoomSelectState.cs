using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomSelectState : MessageStateBase
{
    public RoomSelectState(ServicesContext services, string parentState)
        : base([parentState, StatesList.RoomSelection])
    {
        _regState = new(services, StateUrl);
    }

    private readonly InRoomNameRegistrationState _regState;

    public override Task PrepareState(BotCommandContext context)
    {
        var user = context.User;
        services.Database.Users.UpdateWithState(user, StateUrl);

        var buttons = user.AvailableRooms
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return services.Notifications.SendMessage(user.Id, context.Localization.ChooseRoom, buttons!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, BotCommandContext context)
    {
        var msg = context.Message;
        var user = context.User;

        if (context.State.NextState == InRoomNameRegistrationState.TITLE)
        {
            context.State.MoveToNextState();
            if (await _regState.ProcessMessage(chat, context))
                return true;
        }

        if (msg.Type == MessageType.Command)
            return false;

        if (msg.Type != MessageType.Text)
        {
            await PrepareState(context, []);
            return true;
        }

        var roomId = MessageParser.ParseButtonMessage(msg.TextMsg!.Text).Last();
        var room = user.GetAvailableRoom(roomId);

        if (room is null)
        {
            await services.Notifications.SendErrorMessage(chat.Id, context.Localization.RoomDoesntExist);
            return true;
        }

        user.SelectRoom(room);
        services.Database.Users.UpdateWithClearState(user);

        var participant = user.GetAsParticipant(room);
        if (participant?.RealName is null)
        {
            await _regState.PrepareState(user, []);
            return true;
        }

        await context.GlobalMessageHandler.SendHelpMenu(user);
        return true;
    }
}
