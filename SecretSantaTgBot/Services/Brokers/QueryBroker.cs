using SecretSantaTgBot.Attributes;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.Brokers;

[RequiredService(InjectionType.Transient)]
public class QueryBroker
{
    public Task OnUpdate(Update update)
        => Task.CompletedTask;
}
