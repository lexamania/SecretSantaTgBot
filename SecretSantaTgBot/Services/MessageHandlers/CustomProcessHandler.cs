using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.Messages;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Services.Helpers;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class CustomProcessHandler(NotificationService notifications)
{
    public async Task<bool> HandleCommand(
        MessageStateBase msgState,
        Chat chat,
        MessageContext context)
    {
        if (context.Message.Type != MessageType.Command)
            return false;

        if (msgState.ReadOnlyCommands.TryGetValue(
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
