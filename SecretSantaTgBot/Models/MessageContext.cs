using SecretSantaTgBot.Models.Messages;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models;

public class MessageContext(Message msg, UserEntity user, StateModel state)
{
    public UserEntity User { get; } = user;
    public StateModel State { get; } = state;
    public MessageModel Message { get; } = MessageModel.Build(msg);
}
