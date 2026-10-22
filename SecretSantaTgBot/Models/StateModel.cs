using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Models;

public class StateModel(string state)
{
    private int _position = 0;

    public string State { get; } = state;
    public IReadOnlyList<string> States { get; } = StateUtil.ParseStates(state);
    public IReadOnlyDictionary<string, string> Arguments { get; } = StateUtil.ParseStateArgs(state);

    public string CurrentState => States[_position];
    public string? NextState => _position + 1 < States.Count ? States[++_position] : default;
    public string? PreviousState => _position - 1 >= 0 ? States[--_position] : default;
}
