namespace SecretSanta.TelegramBot.Storage.Entities;

public class UserWishEntity
{
    public string? Message { get; set; }
    public List<string> ImageIds { get; set; } = [];
}
