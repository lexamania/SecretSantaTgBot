using LiteDB;

namespace SecretSanta.Shared.Storage.Entities;

public class ParticipantEntity
{
    [BsonId] public long TelegramUserId { get; set; }
    public string? Username { get; set; }
    public required string CustomName { get; set; }

    public List<UserWishEntity> Wishes { get; set; } = [];
    public long? TargetUserId { get; set; }
}
