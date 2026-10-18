using Microsoft.Extensions.Logging;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Services.Brokers;

[RequiredService(InjectionType.Singleton)]
public class MessageBroker
{
    public MessageBroker(MessageStateContext context, ILogger<MessageBroker> logger)
    {
        context.MessageBroker = this;

        _logger = logger;
        _database = context.Database;
        _notifications = context.Notifications;


        _globalState = new(context);

        _states[DefaultState.TITLE] = new DefaultState(context);
        _states[InRoomCommandState.TITLE] = new InRoomCommandState(context);
    }

    private readonly ILogger<MessageBroker> _logger;
    private readonly SantaDatabase _database;
    private readonly NotificationService _notifications;

    private readonly GlobalState _globalState;
    private readonly Dictionary<string, MessageStateBase> _states = [];

    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = _database.UserDirectory.GetOrCreate(msg.Chat);

        try
        {
            if (await CallMessage(msg, user))
            {
                _logger.LogInformation(msg.Text);
                return;
            }

            _logger.LogWarning(msg.Text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, msg.Text);
        }

        _database.UserDirectory.UpdateWithClearState(user);
        await _notifications.SendErrorCommandMessage(msg.Chat.Id);
        await SendHelpMenu(user);
    }

    public Task SendHelpMenu(UserEntity user)
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
        if (await _globalState.ProcessMessage(msg, user))
            return true;

        var stateStr = GetCurrentState(user);
        return _states.TryGetValue(stateStr, out var state)
            ? await state.ProcessMessage(msg, user)
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
