using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageHandlers;

[RequiredService(InjectionType.Singleton)]
public class CustomProcessHandler
{
    public async Task<bool> HandleStates(MessageStateBase msgState, Chat chat, BotCommandContext context)
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

    public async Task<bool> HandleCommand(MessageStateBase msgState, Chat chat, BotCommandContext context)
    {
        if (msgState.ReadOnlyCommands.TryGetValue(context.Message.CommandMsg!.Command, out var command))
        {
            await command.Execute(chat, context);
            return true;
        }

        return false;
    }
}
