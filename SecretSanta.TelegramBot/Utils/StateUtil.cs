using SecretSanta.TelegramBot.Models.MessageStates.Base;

namespace SecretSanta.TelegramBot.Utils;

public static class StateUtil
{
    public static string[] CalculateStates(IMessageState state)
        => state.Parrent is null
            ? [state.Title]
            : [..CalculateStates(state.Parrent), state.Title];
}
