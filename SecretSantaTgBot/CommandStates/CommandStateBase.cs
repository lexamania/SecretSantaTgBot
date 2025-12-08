using SecretSantaTgBot.Storage.Models;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public abstract class CommandStateBase
{
    public abstract Task OnMessage(Message msg, UserTg user);
}
