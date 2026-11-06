namespace SecretSanta.Shared.Storage.Entities;

public class StateEntity
{
    public string[] States { get; set; } = [];
    public Dictionary<string, string> Arguments { get; set; } = [];
}
