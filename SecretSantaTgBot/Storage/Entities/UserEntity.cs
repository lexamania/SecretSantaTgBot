using LiteDB;

namespace SecretSantaTgBot.Storage.Entities;

public class UserEntity
{
    [BsonId] public long Id { get; set; }
    public string Username { get; set; }

    [BsonRef("rooms")] public PartyRoomEntity? SelectedRoom { get; set; }
    [BsonRef("rooms")] public List<PartyRoomEntity> AvailableRooms { get; set; }

    public string? CurrentState { get; set; }
}
