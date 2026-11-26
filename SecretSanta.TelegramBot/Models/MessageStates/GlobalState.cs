using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;

using Telegram.Bot.Types;
using System.Diagnostics.CodeAnalysis;

namespace SecretSanta.TelegramBot.Models.MessageStates;

public class GlobalState : MessageStateBase
{
    public GlobalState(ServicesContext services)
        : base(StatesList.Global, null, services)
    {
        var defaultState = new DefaultState(services);

        States[StatesList.Default] = defaultState;
        States[StatesList.InRoom] = new InRoomState(services);

        Commands[BotCommandsList.Start] = new StartBotCommand(services, defaultState);
    }

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
        => TryGetMessageState(context.State.States, out var state)
            ? state.SendWelcomeMessage(chat, context)
            : throw new InvalidOperationException("The saved message state path is invalid.");

    public bool IsValidStatePath(string[] states)
        => TryGetMessageState(states, out _);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (context.Message.Type == Messages.MessageType.Command)
        {
            if (Commands.TryGetValue(
                    context.Message.CommandMsg!.Command,
                    out var command))
            {
                await command.Execute(chat, context);
                return true;
            }
        }

        if (TryGetMessageState(context.State.States, out var state))
        {
            if (await state.ProcessMessage(chat, context))
                return true;
        }

        return false;
    }

    private bool TryGetMessageState(
        string[] states,
        [NotNullWhen(true)] out IMessageState? state)
    {
        if (states is not { Length: > 0 })
        {
            state = null;
            return false;
        }

        state = this;
        foreach (var stateName in states)
        {
            if (!state.ReadOnlyStates.TryGetValue(stateName, out var nextState))
            {
                state = null;
                return false;
            }

            state = nextState;
        }

        return true;
    }
}
