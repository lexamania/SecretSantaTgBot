using SecretSantaTgBot.Models;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

public class QueryBrokerService
{
    private readonly ServiceContainer _container;

    public QueryBrokerService (ServiceContainer container)
    {
        container.QueryBroker = this;
        _container = container;
    }

    public async Task OnUpdate(Update update)
    {

    }
}
