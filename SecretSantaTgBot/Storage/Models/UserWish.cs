using LiteDB;

namespace SecretSantaTgBot.Storage.Models;

public class UserWish
{
    public string? Message { get; set; }
    public List<string> Images { get; set; }
}
