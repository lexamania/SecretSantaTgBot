using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class UserWishes
{
    public ObjectId Id { get; set; }
    public string Message { get; set; }
    public string[] ImagePathes { get; set; }
    [BsonRef("users")] public User User { get; set; }
}
