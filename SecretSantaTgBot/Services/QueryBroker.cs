using SecretSantaTgBot.Attributes;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

[RequiredService]
public class QueryBroker
{
    public Task OnUpdate(Update update)
        => Task.CompletedTask;
}
