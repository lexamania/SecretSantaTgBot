using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(MessageStateServiceContext context)
{
    private readonly GlobalState _globalState = new(context);
    private readonly Dictionary<string, MessageStateBase> _states = new()
    {
        [DefaultState.TITLE] = new DefaultState(context),
        [InRoomCommandState.TITLE] = new InRoomCommandState(context)
    };

    public async Task<bool> Handle(Message msg, UserEntity user)
    {
        var userContext = new MessageStateUserContext(user, GetState(user))
        {
            GlobalMessageHandler = this
        };

        if (await _globalState.ProcessMessage(msg, userContext))
            return true;

        return _states.TryGetValue(userContext.State.CurrentState, out var state)
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

    private string GetState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return user.CurrentState;

        return user.SelectedRoom is null
            ? DefaultState.TITLE
            : InRoomCommandState.TITLE;
    }
}
