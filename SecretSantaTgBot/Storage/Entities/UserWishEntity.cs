namespace SecretSantaTgBot.Storage.Entities;

public class UserWishEntity
{
    public string? Message { get; set; }
    public List<string> Images { get; set; } = [];
}
