using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Services;

public class MessageBrokerService
{
    private readonly ServiceContainer _container;
    private readonly GlobalState _globalState;
    private readonly Dictionary<string, MessageStateBase> _states;

    public MessageBrokerService(ServiceContainer container)
    {
        container.MessageBroker = this;
        _container = container;

        _globalState = new(container);
        _states = new()
        {
            [DefaultState.TITLE] = new DefaultState(container),
            [InRoomCommandState.TITLE] = new InRoomCommandState(container),
        };
    }

    public async Task OnMessage(Message msg, UpdateType type)
    {
        var user = _container.Database.UserDirectory.GetOrCreate(msg.Chat);

        try
        {
            if (await CallMessage(msg, user))
            {
                _container.Logger.LogMessage(msg);
                return;
            }

            _container.Logger.LogUnrecognizedMessage(msg);
        }
        catch (Exception ex)
        {
            _container.Logger.LogError(ex);
        }

        _container.Database.UserDirectory.UpdateWithClearState(user);
        await _container.Notification.SendErrorCommandMessage(msg.Chat.Id);
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
