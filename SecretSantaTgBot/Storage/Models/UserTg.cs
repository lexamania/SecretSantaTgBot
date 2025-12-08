using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class UserTg
{
    [BsonId] public long Id { get; set; }
    public string Username { get; set; }

    [BsonRef("rooms")] public PartyRoom? SelectedRoom { get; set; }
    [BsonRef("rooms")] public List<PartyRoom> AvailableRooms { get; set; }

    public string? CurrentState { get; set; }
}
