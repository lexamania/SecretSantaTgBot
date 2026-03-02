using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.InRoomStates;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class RoomSelectState : MessageStateBase
{
    public const string TITLE = "room_selection";
    private readonly InRoomNameRegistrationState _regState;
    private string Message => Msgs.ChooseRoom;

    public RoomSelectState(ServiceContainer container, string parentTitle)
        : base(container, NameParser.JoinArgs(parentTitle, TITLE))
    {
        _regState = new(container, Title);
    }

    public override Task PrepareState(UserEntity user, string[] args)
    {
        UpdateUserState(user, Title);
        var buttons = user.AvailableRooms
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return Notification.SendMessage(user.Id, Message, buttons!);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        var states = NameParser.ParseStateArgs(user.CurrentState, Title);
        if (states.Length > 0 && states[0] == InRoomNameRegistrationState.TITLE)
        {
            if (await _regState!.ProcessMessage(msg, user))
                return true;
        }

        if (MessageParser.IsCommand(msg, out var _, out var _))
            return false;

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var roomId = NameParser.ParseButton(message!).Last();
        var room = user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id.ToString()));

        if (room is null)
        {
            await Notification.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);
            return true;
        }

        user.SelectedRoom = room;
        UpdateUserState(user, default);

        var participant = room.Users.First(u => u.Id == user.Id);
        if (participant.RealName is null)
        {
            await _regState.PrepareState(user, []);
            return true;
        }

        await MessageBroker.SendHelpMenu(user);
        return true;
    }
}
