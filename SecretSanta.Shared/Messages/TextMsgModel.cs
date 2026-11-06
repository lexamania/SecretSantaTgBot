namespace SecretSanta.Shared.Messages;

public class TextMsgModel(string message)
{
    public string Text { get; } = message.Trim();
}
