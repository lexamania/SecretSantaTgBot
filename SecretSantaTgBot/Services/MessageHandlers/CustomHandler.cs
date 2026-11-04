using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Services.Helpers;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class CustomHandler(NotificationService notifications)
{
    public async Task<bool> HandleCommand(
        MessageStateBase state,
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
