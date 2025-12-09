using SecretSantaTgBot.CommandStates;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Services;

public class MessageBrokerService
{
    private readonly Dictionary<string, CommandStateBase> _states;

    public SantaDatabase DB { get; }
    public NotificationService NotifyService { get; }
    public LocalLogger Logger { get; }

    public MessageBrokerService(SantaDatabase db, NotificationService notify, LocalLogger logger)
    {
        DB = db;
        NotifyService = notify;
        Logger = logger;

        _states = new()
        {
            [DefaultState.TITLE] = new DefaultState(this),
            [RoomCreateState.TITLE] = new RoomCreateState(this),
            [RegistrationState.TITLE] = new RegistrationState(this),

            [InRoomCommandState.TITLE] = new InRoomCommandState(this),
        };
    }

    public async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Type != MessageType.Text && msg.Type != MessageType.Photo)
        {
            Logger.LogUnrecognizedMessage(msg);
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id);
            return;
        }

        Logger.LogMessage(msg);
        
        var user = CreateUserIfNeed(msg.Chat);

        try
        {
            await CallMessage(msg, user);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    public Task UpdateAfterStatusChanged(UserTg user)
    {
        var msg = new Message()
        {
            Text = "/help",
            Chat = new()
            {
                Id = user.Id,
                Username = user.Username
            }
        };

        return CallMessage(msg, user);
    }

    private Task CallMessage(Message msg, UserTg user)
    {
        var stateStr = GetCurrentState(user);

        if (!_states.TryGetValue(stateStr, out var state))
            return NotifyService.SendErrorCommandMessage(msg.Chat.Id);
        
        return state.OnMessage(msg, user);
    }

    private string GetCurrentState(UserTg user)
    {
        if (user.CurrentState is not null)
            return NameParser.ParseArgs(user.CurrentState)[0];

        return user.SelectedRoom != null
            ? InRoomCommandState.TITLE
            : DefaultState.TITLE;
    }

    private UserTg CreateUserIfNeed(Chat chat)
    {
        var user = DB.Users
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom)
            .FindById(chat.Id);

        if (user is null)
        {
            user = new() { Id = chat.Id, Username = chat.Username!, AvailableRooms = [] };
            DB.Users.Insert(user);
        }
        else if (user.Username != chat.Username)
        {
            user.Username = chat.Username!;
            DB.Users.Update(user);
        }

        return user;
    }
}
