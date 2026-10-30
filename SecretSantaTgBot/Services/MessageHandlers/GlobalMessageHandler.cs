using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(
    ServicesContext services,
    LocalizationDispatcher localizationDispatcher)
{
    private readonly GlobalState _globalState = new(services);

    public Task<bool> Handle(Message msg, UserEntity user)
    {
        var userContext = new MessageContext(msg, user, ReadState(user))
        {
            GlobalMessageHandler = this,
            Localization = localizationDispatcher.Get(user.Language)
        };

        return _globalState.ProcessMessage(msg.Chat, userContext);
    }

    public Task SendHelpMenu(UserEntity user)
    {
        var msg = new Message()
        {
            Text = "/help",
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
