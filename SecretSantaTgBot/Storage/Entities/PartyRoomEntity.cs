using LiteDB;

namespace SecretSantaTgBot.Storage.Entities;

public class PartyRoomEntity
{
    [BsonId(true)] public Guid Id { get; set; }
    public string Title { get; set; }
    public string PartyDescription { get; set; }
    public bool IsPlayed { get; set; }
    [BsonRef("users")] public UserEntity Admin { get; set; }
    public List<ParticipantEntity> Users { get; set; }
}
