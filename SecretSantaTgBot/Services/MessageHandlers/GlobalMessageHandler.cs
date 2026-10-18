using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler
{
    public GlobalMessageHandler(MessageStateContext context)
    {
        context.GlobalMessageHandler = this;

        _globalState = new(context);

        _states[DefaultState.TITLE] = new DefaultState(context);
        _states[InRoomCommandState.TITLE] = new InRoomCommandState(context);
    }

    private readonly GlobalState _globalState;
    private readonly Dictionary<string, MessageStateBase> _states = [];

    public async Task<bool> Handle(Message msg, UserEntity user)
    {
        if (await _globalState.ProcessMessage(msg, user))
            return true;

        var stateStr = GetCurrentState(user);
        return _states.TryGetValue(stateStr, out var state)
            ? await state.ProcessMessage(msg, user)
            : false;
    }

    public Task SimulateHelpCommand(UserEntity user)
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

        return Handle(msg, user);
    }

    private string GetCurrentState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return StateUtil.ParseState(user.CurrentState)[0];

        return user.SelectedRoom is null
            ? DefaultState.TITLE
            : InRoomCommandState.TITLE;
    }
}
