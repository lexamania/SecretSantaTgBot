using Microsoft.Extensions.DependencyInjection;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates;
using SecretSantaTgBot.Services.Helpers;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class GlobalMessageHandler(
    IServiceProvider provider,
    LocalizationService localizationService)
{
    private GlobalState? _globalState = default;

    public async Task<bool> Handle(Message msg, UserEntity user)
    {
        _globalState ??= new(provider.GetRequiredService<ServicesContext>());
        var language = user.Language;
        var userContext = new MessageContext(msg, user, ReadState(user))
        {
            GlobalMessageHandler = this
        };

        using (localizationService.UseCulture(language))
            return await _globalState.ProcessMessage(msg.Chat, userContext);
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
