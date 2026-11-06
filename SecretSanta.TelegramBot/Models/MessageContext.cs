using SecretSanta.Shared.Models;
using SecretSanta.TelegramBot.Models.Messages;
using SecretSanta.Shared.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models;

public class MessageContext(Message msg, UserEntity user, StateModel state)
{
    public UserEntity User { get; } = user;
    public StateModel State { get; } = state;
    public MessageModel Message { get; } = MessageModel.Build(msg);
}
