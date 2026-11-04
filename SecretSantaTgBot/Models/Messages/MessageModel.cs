using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Messages;

public enum MessageType
{
    Default = 0,
    Command = 1,
    Text = 2,
    Image = 3
}

public class MessageModel
{
    public MessageType Type { get; }

    public CommandMsgModel? CommandMsg { get; }
    public TextMsgModel? TextMsg { get; }
    public ImageMsgModel? ImageMsg { get; }

    private MessageModel(string command, string[] args)
    {
        Type = MessageType.Command;
        CommandMsg = new(command, args);
    }

    private MessageModel(string text)
    {
        Type = MessageType.Text;
        TextMsg = new(text!);
    }

    private MessageModel(string text, PhotoSize image)
    {
        Type = MessageType.Image;
        ImageMsg = new(image!.FileId, text);
    }

    private MessageModel()
    {
        Type = MessageType.Default;
    }

    public static MessageModel Build(Message msg)
    {
        if (MessageParser.TryParseCommand(msg, out var command, out var args))
            return new(command!, args!);

        if (MessageParser.TryParseText(msg, out var text))
            return new(text!);

        if (MessageParser.TryParseImage(msg, out var caption, out var image))
            return new(caption!, image!);

        return new();
    }
}
