using LiteDB;

namespace SecretSantaTgBot.Storage.Entities;

public class ParticipantEntity
{
    [BsonId] public long Id { get; set; }
    public string Username { get; set; }
    public string? RealName { get; set; }

    public List<UserWishEntity> Wishes { get; set; } = [];
    public long? TargetUserId { get; set; }
}
