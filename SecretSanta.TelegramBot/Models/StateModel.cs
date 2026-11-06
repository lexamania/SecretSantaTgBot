using System.Text.Json;

namespace SecretSanta.TelegramBot.Models;

public class StateModel
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public static StateModel Build(string state)
        => JsonSerializer.Deserialize<StateModel>(state, Options)!;

    public override string ToString()
        => JsonSerializer.Serialize(this, Options);

    public string[] States { get; set; } = [];
    public Dictionary<string, string> Arguments { get; set; } = [];
}
