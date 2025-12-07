using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class Participant
{
    [BsonRef("users")] public User User { get; set; }
    [BsonRef("users")] public User? TargetUser { get; set; }
    public bool IsActive { get; set; }
}
