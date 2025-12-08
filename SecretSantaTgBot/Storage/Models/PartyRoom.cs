using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class PartyRoom
{
    [BsonId(true)] public Guid Id { get; set; }
    public string Title { get; set; }
    public string PartyDescription { get; set; }
    public bool IsPlayed { get; set; }
    [BsonRef("users")] public UserTg Admin { get; set; }
    public List<Participant> Users { get; set; }
}