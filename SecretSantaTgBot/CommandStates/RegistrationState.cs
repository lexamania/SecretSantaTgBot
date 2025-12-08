using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RegistrationState  : CommandStateBase
{
    public const string TITLE = "registration";

    private readonly SantaDatabase _db;
    private readonly NotificationService _notifyService;
    private readonly MessageBrokerService _csm;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public RegistrationState(MessageBrokerService csm)
    {
        _csm = csm;
        _db = csm.DB;
        _notifyService = csm.NotifyService;
    }

    public override async Task OnMessage(Message msg, UserTg user)
    {
        var args = NameParser.ParseArgs(user.CurrentState!);

        if (msg.Text is not { Length: > 0 } || msg.Text.StartsWith('/'))
        {
            await _notifyService.SendErrorCommandMessage(msg.Chat.Id, Msgs.EnterRealName);
            return;
        }

        if (args.Length < 2)
        {
            user.CurrentState = default;
            _db.Users.Update(user);
            await _notifyService.SendErrorCommandMessage(msg.Chat.Id, Msgs.EnterRealName);
            return;
        }

        var roomId = new Guid(args[1]);
        var room = _db.Rooms.FindById(roomId);

        if (!user.AvailableRooms.Contains(room))
            user.AvailableRooms.Add(room);

        room.Users.Add(new()
        {
            Id = user.Id,
            Username = user.Username,
            RealName = msg.Text!.Trim()
        });

        user.CurrentState = default;
        _db.Users.Update(user);
        _db.Rooms.Update(room);

        await _notifyService.SendMessage(msg.Chat.Id, Msgs.UserParticipationEnd);
        await _csm.UpdateAfterStatusChanged(user);
    }
}
