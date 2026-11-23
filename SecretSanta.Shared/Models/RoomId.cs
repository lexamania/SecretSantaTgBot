namespace SecretSanta.Shared.Models;

public readonly record struct RoomId(Guid Value)
{
    public static bool TryParse(string? value, out RoomId roomId)
    {
        if (Guid.TryParse(value, out var guid))
        {
            roomId = new RoomId(guid);
            return true;
        }

        roomId = default;
        return false;
    }

    public override string ToString() => Value.ToString("D");
}
