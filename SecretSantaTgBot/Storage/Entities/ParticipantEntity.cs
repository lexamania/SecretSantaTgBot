using LiteDB;

namespace SecretSantaTgBot.Storage.Entities;

public class ParticipantEntity
{
    [BsonId] public long TelegramUserId { get; set; }
    public string? Username { get; set; }
    public required string InRoomName { get; set; }

    public List<UserWishEntity> Wishes { get; set; } = [];
    public long? TargetUserId { get; set; }
}
