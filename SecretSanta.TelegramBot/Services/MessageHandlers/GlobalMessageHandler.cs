using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Models;
using SecretSanta.TelegramBot.Models.MessageStates;
using SecretSanta.Shared.Storage;
using SecretSanta.Shared.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(
    IServiceProvider provider,
    ISantaDatabase database,
    ILogger<GlobalMessageHandler> logger)
{
    private readonly Lazy<GlobalState> _globalState = new(() =>
        new(provider.GetRequiredService<ServicesContext>()));

    public GlobalState GlobalState => _globalState.Value;

    public async Task<bool> HandleMessage(Message msg, UserEntity user)
    {
        var userContext = new MessageContext(msg, user, await ReadStateAsync(user));
        return await GlobalState.ProcessMessage(msg.Chat, userContext);
    }

    public async Task ResetStateAsync(
        Chat chat,
        long telegramUserId,
        bool sendWelcomeMessage = true,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await database.Users.GetByIdAsync(
            telegramUserId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                $"User {telegramUserId} was not found while resetting their state.");
        await database.Users.ClearStateAsync(currentUser, cancellationToken);

        if (!sendWelcomeMessage)
            return;

        var userContext = new MessageContext(
            new() { Chat = chat },
            currentUser,
            CreateMenuState(currentUser));
        await GlobalState.SendWelcomeMessage(chat, userContext);
    }

    private async Task<StateModel> ReadStateAsync(UserEntity user)
    {
        if (user.CurrentState is not null)
        {
            if (StateModel.TryBuild(user.CurrentState, out var state)
                && GlobalState.IsValidStatePath(state.States))
            {
                return state;
            }

            logger.LogWarning(
                "Resetting invalid saved state for Telegram user {TelegramUserId}.",
                user.TelegramUserId);

            await database.Users.ClearStateAsync(user);
        }

        return CreateMenuState(user);
    }

    private static StateModel CreateMenuState(UserEntity user)
        => new()
        {
            States = [
                user.SelectedRoom is null
                    ? StatesList.Default
                    : StatesList.InRoom
            ]
        };
}
