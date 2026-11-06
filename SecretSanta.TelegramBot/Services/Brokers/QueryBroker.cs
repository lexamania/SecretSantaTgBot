using SecretSanta.TelegramBot.Attributes;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Services.Brokers;

[RequiredService(InjectionType.Transient)]
public class QueryBroker
{
    public Task OnUpdate(Update update)
        => Task.CompletedTask;
}
