using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Models;

public class StateObject
{
    public StateObject(string state)
    {
        OriginalState = state;
        (States, Arguments) = StateUtil.Parse(state);
    }

    public string OriginalState { get; private set; }
    public List<string> States { get; private set; }
    public Dictionary<string, string> Arguments { get; private set; }

    public void RestoreOriginal()
    {
        (States, Arguments) = StateUtil.Parse(OriginalState);
    }

    public override string ToString()
        => StateUtil.Join(States, Arguments);
}

public class StateModel(string state)
{
    private int _position = 0;

    public StateObject StateObject { get; private set; } = new(state);

    public string CurrentState => StateObject.States[_position];
    
    public string? NextState => _position < StateObject.States.Count - 1
        ? StateObject.States[_position + 1]
        : default;

    public string? PreviousState => _position > 0
        ? StateObject.States[_position - 1]
        : default;

    public void MoveToNextState()
    {
        if (_position == StateObject.States.Count - 1)
            return;

        ++_position;
    }

    public void ReplaceState(string state)
    {
        _position = 0;
        StateObject = new(state);
    }
}
