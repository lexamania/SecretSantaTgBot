using LiteDB;

using SecretSantaTgBot.Localization;

namespace SecretSantaTgBot.Storage.Entities;

public class UserEntity
{
    [BsonId] public long Id { get; set; }
    public string Username { get; set; }
    public string Language { get; set; } = LocalizationDispatcher.DEFAULT_LANG;

    [BsonRef("rooms")] public PartyRoomEntity? SelectedRoom { get; set; }
    [BsonRef("rooms")] public List<PartyRoomEntity> AvailableRooms { get; set; } = [];

    public string? CurrentState { get; set; }
}
