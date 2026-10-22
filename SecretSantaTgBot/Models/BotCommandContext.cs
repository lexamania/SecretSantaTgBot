using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Models;

public class BotCommandContext(UserEntity user, string state)
{
    public UserEntity User { get; } = user;
    public StateModel State { get; } = new(state);

    public required GlobalMessageHandler GlobalMessageHandler { get; init; }
}
