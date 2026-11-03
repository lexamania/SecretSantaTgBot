using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class GlobalState : MessageStateBase
{
    public GlobalState(ServicesContext services)
        : base(services, StatesList.Global)
    {
        var defaultState = new DefaultState(services);

        States[StatesList.Default] = defaultState;
        States[StatesList.InRoom] = new InRoomState(services);

        Commands[BotCommandsList.Start] = new StartBotCommand(services, defaultState);
    }

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

        if (TryGetMessageState(this, context.State, out var state))
        {
            if (await state.ProcessMessage(chat, context))
                return true;
        }

        return false;
    }

    private bool TryGetMessageState(
        MessageStateBase parrentState,
        StateModel stateModel,
        out MessageStateBase state)
    {
        if (!parrentState.ReadOnlyStates.TryGetValue(
                stateModel.CurrentState,
                out var currentState))
        {
            state = null;
            return false;
        }

        if (stateModel.NextState is null)
        {
            state = currentState;
            return true;
        }
        
        stateModel.MoveToNextState();
        return TryGetMessageState(currentState, stateModel, out state);
    }
}
