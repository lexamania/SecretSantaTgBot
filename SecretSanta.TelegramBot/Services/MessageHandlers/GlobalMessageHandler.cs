using Microsoft.Extensions.DependencyInjection;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Models;
using SecretSanta.TelegramBot.Models.MessageStates;
using SecretSanta.Shared.Storage;
using SecretSanta.Shared.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(IServiceProvider provider, ISantaDatabase database)
{
    private readonly Lazy<GlobalState> _globalState = new(() =>
        new(provider.GetRequiredService<ServicesContext>()));

    public GlobalState GlobalState => _globalState.Value;

    public Task<bool> HandleMessage(Message msg, UserEntity user)
    {
        var userContext = new MessageContext(msg, user, ReadState(user));
        return GlobalState.ProcessMessage(msg.Chat, userContext);
    }

    public async Task ExitState(Chat chat, UserEntity user, bool withMessage = true)
    {
        await database.Users.ClearStateAsync(user);

        if (!withMessage)
            return;

        var userContext = new MessageContext(new() { Chat = chat }, user, ReadState(user));
        await GlobalState.SendWelcomeMessage(chat, userContext);
    }

    private StateModel ReadState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return StateModel.Build(user.CurrentState);

        return new StateModel()
        {
            States = [
                user.SelectedRoom is null
                    ? StatesList.Default
                    : StatesList.InRoom
            ]
        };
    }
}
