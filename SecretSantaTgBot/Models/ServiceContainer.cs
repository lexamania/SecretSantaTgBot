using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

namespace SecretSantaTgBot.Models;

public record ServiceContainer (
    SantaDatabase Database,
    NotificationService Notification,
    LocalLogger Logger
)
{
    public MessageBrokerService? MessageBroker { get; set; }
    public QueryBrokerService? QueryBroker { get; set; }
};
