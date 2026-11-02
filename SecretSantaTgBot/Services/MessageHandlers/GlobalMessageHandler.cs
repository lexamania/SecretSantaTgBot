using Microsoft.Extensions.DependencyInjection;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(IServiceProvider provider)
{
    private readonly Lazy<GlobalState> _globalState = new(() => 
        new(provider.GetRequiredService<ServicesContext>()));

    public GlobalState GlobalState => _globalState.Value;

    public Task<bool> Handle(Message msg, UserEntity user)
    {
        var userContext = new MessageContext(msg, user, ReadState(user));
        return GlobalState.ProcessMessage(msg.Chat, userContext);
    }

    public Task SendHelpMenu(UserEntity user)
    {
        var msg = new Message()
        {
            Text = BotCommandsList.Help,
            Chat = new()
            {
                Id = user.TelegramUserId,
                Username = user.Username
            }
        };

        return Handle(msg, user);
    }

    private string ReadState(UserEntity user)
    {
        if (user.CurrentState is not null)
            return user.CurrentState;

        return user.SelectedRoom is null
            ? StatesList.Default
            : StatesList.InRoom;
    }
}
