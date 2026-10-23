using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Models;

public class StateModel(string state)
{
    private int _position = 0;

    public string StateUrl { get; } = state;
    public IReadOnlyList<string> States { get; } = StateUtil.ParseStates(state);
    public IReadOnlyDictionary<string, string> Arguments { get; } = StateUtil.ParseStateArgs(state);

    public string CurrentState => States[_position];
    public string? NextState => _position < States.Count - 1 ? States[_position + 1] : default;
    public string? PreviousState => _position > 0 ? States[_position - 1] : default;

    public void MoveToNextState()
    {
        if (_position == States.Count - 1)
            return;

        ++_position;
    }
}
