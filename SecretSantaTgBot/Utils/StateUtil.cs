using SecretSantaTgBot.Models.MessageStates.Base;

namespace SecretSantaTgBot.Utils;

public static class StateUtil
{
    public static string[] CalculateStates(MessageStateBase state)
        => state.ParrentState is null
            ? [state.Title]
            : [..CalculateStates(state.ParrentState), state.Title];
}
