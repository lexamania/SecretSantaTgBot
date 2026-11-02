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
    public Task<bool> HandleGlobalState(
        MessageStateBase msgState,
        Chat chat,
        MessageContext context)
    {
        return msgState.ReadOnlyStates.TryGetValue(
                context.State.CurrentState,
                out var state)
            ? state.ProcessMessage(chat, context)
            : Task.FromResult(false);
    }

    public async Task<bool> HandleState(
        MessageStateBase msgState,
        Chat chat,
        MessageContext context)
    {
        if (context.State.NextState is not { } strState)
            return false;

        if (msgState.ReadOnlyStates.TryGetValue(
                strState,
                out var state))
        {
            context.State.MoveToNextState();
            if (await state.ProcessMessage(chat, context))
                return true;
        }

        await notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.ErrServer);
        return true;
    }

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
