using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

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
        => TryGetMessageState(this, context.State.States, out var state)
            ? state.SendWelcomeMessage(chat, context)
            : Task.CompletedTask;

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

        if (TryGetMessageState(this, context.State.States, out var state))
        {
            if (await state.ProcessMessage(chat, context))
                return true;
        }

        return false;
    }

    private bool TryGetMessageState(
        IMessageState parrent,
        string[] states,
        out IMessageState state,
        int iterator = 0)
    {
        if (!parrent.ReadOnlyStates.TryGetValue(
                states[iterator],
                out var currentState))
        {
            state = null;
            return false;
        }

        if (iterator == states.Length - 1)
        {
            state = currentState;
            return true;
        }

        return TryGetMessageState(currentState, states, out state, iterator + 1);
    }
}
