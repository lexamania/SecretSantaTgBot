using LiteDB;

using SecretSanta.Shared.Models;

namespace SecretSanta.Shared.Storage.Entities;

public class PartyRoomEntity
{
    [BsonId(true)] public Guid IdValue { get; set; }
    [BsonIgnore] public RoomId Id
    {
        get => new(IdValue);
        set => IdValue = value.Value;
    }

    public required string Title { get; set; }
    public required string Description { get; set; }
    public long AdminTelegramUserId { get; set; }
    public List<ParticipantEntity> Participants { get; set; } = [];
    public bool SantaPlayed { get; set; }
}
