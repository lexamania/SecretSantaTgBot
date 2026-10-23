using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(
    ServicesContext context,
    LocalizationDispatcher localizationDispatcher)
{
    private readonly GlobalState _globalState = new(context);
    private readonly Dictionary<string, MessageStateBase> _states = new()
    {
        [StatesList.Default] = new DefaultState(context),
        [StatesList.InRoom] = new InRoomCommandState(context)
    };

    public async Task<bool> Handle(Message msg, UserEntity user)
    {
        var userContext = new BotCommandContext(msg, user, ReadState(user))
        {
            GlobalMessageHandler = this,
            Localization = localizationDispatcher.Get(user.Language)
        };

        if (await _globalState.ProcessMessage(msg.Chat, userContext))
            return true;

        return _states.TryGetValue(userContext.State.CurrentState, out var state)
            ? await state.ProcessMessage(msg.Chat, userContext)
            : false;
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

        return Handle(msg, user);
    }

    private string ReadState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return user.CurrentState;

        return user.SelectedRoom is null
            ? StatesList.Default
            : StatesList.InRoom;
    }
}
