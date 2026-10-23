using SecretSantaTgBot.Models.MessageStates.Base;

namespace SecretSantaTgBot.Utils;

public static class StateUtil
{
    private const char Separetor = '/';
    private const char ArgsEnter = '?';
    private const string ArgsSeparator = "&&";

    public static string JoinStates(params string[] states)
        => string.Join(Separetor, states);

    public static Dictionary<string, string> ParseStateArgs(string state)
        => state.Contains(ArgsEnter)
            ? state.Split(ArgsEnter).Last()
                .Split(ArgsSeparator)
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1])
            : [];

    public static string[] ParseStates(string state)
        => [.. state
            .Split(ArgsEnter).First()
            .Split(Separetor).Where(x => x.Length > 0)];
    public static string[] ParseState(string state)
        => [.. state.Split(Separetor).Where(x => x.Length > 0)];

    public static string[] ParseState(string? state, string current)
        => state is not null
            ? ParseState(ClearState(state, current)!)
            : [];

    public static string? ClearState(string? state, string current)
    {
        if (state is null)
            return null;

        if (!state.StartsWith(current))
            return state;

        return state.Length > current.Length 
            ? state[current.Length..]
            : string.Empty;
    }

    public static bool HasNewState(
        string state,
        string currentState,
        Dictionary<string, MessageStateBase> availableStates, 
        out MessageStateBase? result)
    {
        result = default;

        var states = ParseState(state, currentState);
        return states.Length != 0 && availableStates.TryGetValue(states[0], out result);
    }
}
