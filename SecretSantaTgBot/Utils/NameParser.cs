namespace SecretSantaTgBot.Utils;

public static class NameParser
{
    public static string[] ParseArgs(string text)
        => text.Split(' ').Where(x => x.Length > 0).ToArray();

    public static string[] ParseStateArgs(string? state, string current)
        => state is not null
            ? ParseArgs(ClearState(state, current)!)
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

    public static string[] ParseButton(string text)
        => text.Split().Where(x => x.Length > 0).ToArray();

    public static string JoinArgs(params object[] args)
        => string.Join(' ', args);
}
