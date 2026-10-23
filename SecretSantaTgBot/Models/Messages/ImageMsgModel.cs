namespace SecretSantaTgBot.Models.Messages;

public class ImageMsgModel(string imageId, string? text)
{
    public string ImageId { get; } = imageId;
    public string? Text { get; } = text?.Trim();
}
