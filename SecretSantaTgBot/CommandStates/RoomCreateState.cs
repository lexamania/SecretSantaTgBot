using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RoomCreateState : CommandStateBase
{
    public const string TITLE = "room_creation";

    private readonly SantaDatabase _db;
    private readonly NotificationService _notifyService;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public RoomCreateState(MessageBrokerService csm)
    {
        _db = csm.DB;
        _notifyService = csm.NotifyService;
    }

    public override async Task OnMessage(Message msg, UserTg user)
    {
        var stateArgs = NameParser.ParseArgs(user.CurrentState!);

        var roomId = stateArgs.Length > 1 ? stateArgs[1] : null;
        var helpMessage = roomId == null
            ? Msgs.RoomCreationEnterTitle
            : Msgs.RoomCreationEnterDescription;

        if (msg.Text is not { Length: > 0 } || msg.Text.StartsWith('/'))
        {
            await _notifyService.SendErrorCommandMessage(msg.Chat.Id, helpMessage);
            return;
        }

        var text = msg.Text!.Trim();
        var task = roomId == null
            ? OnTitleEnter(user, text)
            : OnDescriptionEnter(user, text, roomId);

        await task;
    }

    private Task OnTitleEnter(UserTg user, string msg)
    {
        var room = new PartyRoom()
        {
            Title = msg,
            Admin = user,
            Users = []
        };

        _db.Rooms.Insert(room);

        user.CurrentState = NameParser.JoinArgs(TITLE, room.Id);
        _db.Users.Update(user);

        return _notifyService.SendMessage(user.Id, Msgs.RoomCreationEnterDescription);
    }

    private async Task OnDescriptionEnter(UserTg user, string text, string roomId)
    {
        var room = _db.Rooms.FindById(new Guid(roomId));
        room.PartyDescription = text;
        _db.Rooms.Update(room);

        user.CurrentState = NameParser.JoinArgs(RegistrationState.TITLE, roomId);
        _db.Users.Update(user);

        var message = MessageBuilder.BuildCreateRoomMessage(roomId);
        await _notifyService.SendMessage(user.Id, message);
        await _notifyService.SendMessage(user.Id, Msgs.EnterRealName);
    }
}
