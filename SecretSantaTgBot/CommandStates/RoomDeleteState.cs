using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RoomDeleteState : CommandStateBase
{
    public const string TITLE = "room_delete";

    private readonly SantaDatabase _db;
    private readonly NotificationService _notifyService;
    private readonly MessageBrokerService _csm;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public RoomDeleteState(MessageBrokerService csm)
    {
        _csm = csm;
        _db = csm.DB;
        _notifyService = csm.NotifyService;
    }

    public override async Task OnMessage(Message msg, UserTg user)
    {
        if (msg.Text is not { Length: > 0 } || msg.Text.StartsWith('/'))
        {
            await _notifyService.SendErrorCommandMessage(msg.Chat.Id, Msgs.ChooseRoom);
            return;
        }
    
        var text = msg.Text!.Trim();
        var roomId = NameParser.ParseButton(text).Last();
        var room = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
        {
            await _notifyService.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);
            return;
        }

        var userIds = room.Users.Select(x => x.Id).ToList();
        var users = _db.Users
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom)
            .Find(x => userIds.Contains(x.Id))
            .ToList();

        foreach (var u in users)
        {
            u.AvailableRooms.Remove(room);
            if (u.SelectedRoom?.Id == room.Id)
            {
                u.SelectedRoom = default;
                u.CurrentState = default;
            }
        }

        _db.Rooms.Delete(room.Id);
        _db.Users.Update(users);

        var message = MessageBuilder.BuildDeleteRoomMessage(room);
        await _notifyService.SendMessage(msg.Chat.Id, message);
        await _csm.UpdateAfterStatusChanged(user);
    }
}
