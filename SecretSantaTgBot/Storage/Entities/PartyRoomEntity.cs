using LiteDB;

namespace SecretSantaTgBot.Storage.Entities;

public class PartyRoomEntity
{
    [BsonId(true)] public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    [BsonRef("users")] public required UserEntity Admin { get; set; }
    public List<ParticipantEntity> Participants { get; set; } = [];
    public bool SantaPlayed { get; set; }
}
