using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class Participant
{
    [BsonId] public long Id { get; set; }
    public string Username { get; set; }
    public string? RealName { get; set; }

    public List<UserWish> Wishes { get; set; } 
    [BsonRef("users")] public UserTg TargetUser { get; set; }

    public string? CurrentState { get; set; }
    public string? LastCommand { get; set; }
}
