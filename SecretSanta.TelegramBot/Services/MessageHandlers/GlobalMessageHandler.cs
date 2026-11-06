using Microsoft.Extensions.DependencyInjection;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models;
using SecretSanta.TelegramBot.Models.MessageStates;
using SecretSanta.TelegramBot.Storage;
using SecretSanta.TelegramBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(IServiceProvider provider)
{
    private readonly Lazy<GlobalState> _globalState = new(() =>
        new(provider.GetRequiredService<ServicesContext>()));

    public GlobalState GlobalState => _globalState.Value;

    public Task<bool> HandleMessage(Message msg, UserEntity user)
    {
        var userContext = new MessageContext(msg, user, ReadState(user));
        return GlobalState.ProcessMessage(msg.Chat, userContext);
    }

    public Task ExitState(Chat chat, UserEntity user, bool withMessage = true)
    {
        var database = provider.GetRequiredService<SantaDatabase>();
        database.Users.ClearState(user);

        if (!withMessage)
            return Task.CompletedTask;

        var userContext = new MessageContext(new() { Chat = chat }, user, ReadState(user));
        return GlobalState.SendWelcomeMessage(chat, userContext);
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
