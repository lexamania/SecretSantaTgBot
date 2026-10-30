using SecretSantaTgBot.Localization.Dictionaries;
using SecretSantaTgBot.Models.Messages;
using SecretSantaTgBot.Services.MessageHandlers;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models;

public class MessageContext(Message msg, UserEntity user, string state)
{
    public UserEntity User { get; } = user;
    public StateModel State { get; } = new(state);
    public MessageModel Message { get; } = new(msg);

    public required GlobalMessageHandler GlobalMessageHandler { get; init; }
}
