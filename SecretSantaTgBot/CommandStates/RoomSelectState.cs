
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RoomSelectState : CommandStateBase
{
    public const string TITLE = "room_selection";

    private readonly SantaDatabase _db;
    private readonly NotificationService _notifyService;
    private readonly MessageBrokerService _csm;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public RoomSelectState(MessageBrokerService csm)
    {
        _csm = csm;
        _db = csm.DB;
        _notifyService = csm.NotifyService;
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        if (msg.Text is not { Length: > 0 } || msg.Text.StartsWith('/'))
            return _notifyService.SendErrorCommandMessage(msg.Chat.Id, Msgs.ChooseRoom);

        var text = msg.Text!.Trim();

        var roomId = NameParser.ParseButton(text).Last();
        var room = user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id.ToString()));

        if (room is null)
            return _notifyService.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);

        user.SelectedRoom = room;
        user.CurrentState = default;
        _db.Users.Update(user);

        return _csm.UpdateAfterStatusChanged(user);
    }
}
