using SecretSantaTgBot.Models.MessageStates.Base;

namespace SecretSantaTgBot.Utils;

public static class StateUtil
{
    public static string[] CalculateStates(MessageStateBase state)
        => state.Parrent is null
            ? [state.Title]
            : [..CalculateStates(state.Parrent), state.Title];
}
