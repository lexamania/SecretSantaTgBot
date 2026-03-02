using SecretSantaTgBot.Services.MessageStates;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Services;

public class MessageBrokerService
{
    private readonly Dictionary<string, MessageStateBase> _states;
    private readonly GlobalState _globalState;

    public SantaDatabase DB { get; }
    public NotificationService NotifyService { get; }
    public LocalLogger Logger { get; }

    public MessageBrokerService(SantaDatabase db, NotificationService notify, LocalLogger logger)
    {
        DB = db;
        NotifyService = notify;
        Logger = logger;

        _globalState = new(this);
        _states = new()
        {
            [DefaultState.TITLE] = new DefaultState(this),
            [InRoomCommandState.TITLE] = new InRoomCommandState(this),
        };
    }

    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = DB.UserDirectory.GetOrCreate(msg.Chat);

        try
        {
            if (await CallMessage(msg, user))
            {
                Logger.LogMessage(msg);
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            return;
        }

        DB.UserDirectory.UpdateWithClearState(user);

        Logger.LogUnrecognizedMessage(msg);
        await NotifyService.SendErrorCommandMessage(msg.Chat.Id);
    }

    public Task UpdateAfterStatusChanged(UserEntity user)
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

    private async Task<bool> CallMessage(Message msg, UserEntity user)
    {
        if (await _globalState.OnMessage(msg, user))
            return true;

        var stateStr = GetCurrentState(user);
        return _states.TryGetValue(stateStr, out var state) 
            ? await state.OnMessage(msg, user)
            : false;
    }

    private string GetCurrentState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return NameParser.ParseStateArgs(user.CurrentState, "abcd")[0];

        return user.SelectedRoom != null
            ? InRoomCommandState.TITLE
            : DefaultState.TITLE;
    }
}
