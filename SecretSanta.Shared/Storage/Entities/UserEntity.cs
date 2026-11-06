using LiteDB;

namespace SecretSanta.Shared.Storage.Entities;

public class UserEntity
{
    [BsonId] public long TelegramUserId { get; set; }
    public string? Username { get; set; }
    public string? PrefferedLanguage { get; set; }

    [BsonRef("rooms")] public PartyRoomEntity? SelectedRoom { get; set; }
    [BsonRef("rooms")] public List<PartyRoomEntity> AvailableRooms { get; set; } = [];

    public string? CurrentState { get; set; }
}
