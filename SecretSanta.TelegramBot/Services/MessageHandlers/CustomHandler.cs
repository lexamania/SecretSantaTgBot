using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Models;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Services.Helpers;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class CustomHandler(NotificationService notifications)
{
    public async Task<bool> HandleCommand(
        IMessageState state,
        Chat chat,
        MessageContext context)
    {
        if (context.Message.Type != Models.Messages.MessageType.Command)
            return false;

        if (state.ReadOnlyCommands.TryGetValue(
                context.Message.CommandMsg!.Command,
                out var command))
        {
            await command.Execute(chat, context);
            return true;
        }

        await notifications.SendStagedMessageByKey(
            chat.Id, BotContent.Messages.ErrCommand);
        return true;
    }
}
