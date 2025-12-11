using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.InRoomStates;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class RoomSelectState : SimpleMessageStateBase
{
    public const string TITLE = "room_selection";
    private readonly InRoomNameRegistrationState _regState;

    public RoomSelectState(MessageBrokerService csm, string parentTitle) : base(csm, NameParser.JoinArgs(parentTitle, TITLE))
    {
        _regState = new(csm, Title);
    }

    protected override string Message => Msgs.ChooseRoom;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        var states = NameParser.ParseStateArgs(user.CurrentState, Title);
        if (states.Length > 0 && states[0] == InRoomNameRegistrationState.TITLE)
        {
            if (await _regState!.OnMessage(msg, user))
                return true;
        }

        if (MessageParser.IsCommand(msg, out var _, out var _))
            return false;

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var roomId = NameParser.ParseButton(message!).Last();
        var room = user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id.ToString()));

        if (room is null)
        {
            await NotifyService.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);
            return true;
        }

        user.SelectedRoom = room;
        UpdateUserState(user, default);

        var participant = room.Users.First(u => u.Id == user.Id);
        if (participant.RealName is null)
        {
            await _regState.StartState(user, []);
            return true;
        }

        await Csm.UpdateAfterStatusChanged(user);
        return true;
    }

    public override Task StartState(UserTg user, string[] args)
    {
        UpdateUserState(user, Title);
        var buttons = user.AvailableRooms
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return NotifyService.SendMessage(user.Id, Message, buttons!);
    }
}
