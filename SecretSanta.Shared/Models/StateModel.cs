using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SecretSanta.Shared.Models;

public class StateModel
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public static bool TryBuild(
        string? state,
        [NotNullWhen(true)] out StateModel? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(state))
            return false;

        try
        {
            var model = JsonSerializer.Deserialize<StateModel>(state, Options);
            if (model is null
                || model.States is not { Length: > 0 }
                || model.States.Any(string.IsNullOrWhiteSpace)
                || model.Arguments is null
                || model.Arguments.Any(argument => argument.Value is null))
            {
                return false;
            }

            result = model;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public override string ToString()
        => JsonSerializer.Serialize(this, Options);

    public string[] States { get; set; } = [];
    public Dictionary<string, string> Arguments { get; set; } = [];
}
