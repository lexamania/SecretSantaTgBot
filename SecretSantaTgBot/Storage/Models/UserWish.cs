using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class UserWish
{
    [BsonId(true)] public ObjectId Id { get; set; }
    public string Message { get; set; }
    public string[]? Images { get; set; }
    public long UserId { get; set; }
}
