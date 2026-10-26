using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Messages;

public enum MessageType
{
    Command = 0,
    Text = 1,
    Image = 2
}

public class MessageModel
{
    public MessageType Type { get; }

    public CommandMsgModel? CommandMsg { get; }
    public TextMsgModel? TextMsg { get; }
    public ImageMsgModel? ImageMsg { get; }

    public MessageModel(Message msg)
    {
        if (MessageParser.TryParseCommand(msg, out var command, out var args))
        {
            CommandMsg = new(command!, args!);
            Type = MessageType.Command;
            return;
        }
        if (MessageParser.TryParseText(msg, out var text))
        {
            TextMsg = new(text!);
            Type = MessageType.Text;
            return;
        }
        if (MessageParser.TryParseImage(msg, out var caption, out var image))
        {
            ImageMsg = new(image!.FileId, caption);
            Type = MessageType.Image;
            return;
        }
    }
}
