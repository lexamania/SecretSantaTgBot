using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.Messages;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class CustomProcessHandler
{
    public async Task<bool> HandleGlobalState(MessageStateBase msgState, Chat chat, MessageContext context)
    {
        if (context.State.CurrentState is { } strState)
        {
            if (msgState.ReadOnlyStates.TryGetValue(strState, out var state))
            {
                if (await state.ProcessMessage(chat, context))
                    return true;
            }
        }

        return false;
    }
    public async Task<bool> HandleState(MessageStateBase msgState, Chat chat, MessageContext context)
    {
        if (context.State.NextState is { } strState)
        {
            if (msgState.ReadOnlyStates.TryGetValue(strState, out var state))
            {
                context.State.MoveToNextState();
                if (await state.ProcessMessage(chat, context))
                    return true;
            }
        }

        return false;
    }

    public async Task<bool> HandleCommand(MessageStateBase msgState, Chat chat, MessageContext context)
    {
        if (context.Message.Type == MessageType.Command && 
            msgState.ReadOnlyCommands.TryGetValue(context.Message.CommandMsg!.Command, out var command))
        {
            await command.Execute(chat, context);
            return true;
        }

        return false;
    }
}
