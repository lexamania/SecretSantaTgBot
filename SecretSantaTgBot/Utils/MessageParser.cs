using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Utils;

public static class MessageParser
{
    public static string[] ParseButtonMessage(string text)
        => [.. text.Split().Where(x => x.Length > 0)];

    public static bool TryParseCommand(Message msg, out string? command, out string[]? args)
    {
        var text = msg.Text?.Trim();
        if (msg.Type == MessageType.Text && text!.StartsWith('/'))
        {
            var words = text!.Split().Where(x => x.Length > 0).ToArray();
            command = words[0];
            args = words.Length > 1
                ? words[1..]
                : [];

            return true;
        }

        command = default;
        args = default;
        return false;
    }

    public static bool TryParseText(Message msg, out string? message)
    {
        var text = msg.Text?.Trim();
        if (msg.Type == MessageType.Text && !text!.StartsWith('/'))
        {
            message = text;
            return true;
        }

        message = default;
        return false;
    }

    public static bool TryParseImage(Message msg, out string? message, out PhotoSize? image)
    {
        if (msg.Type == MessageType.Photo)
        {
            message = msg.Caption?.Trim();
            image = msg.Photo!.First();
            return true;
        }

        message = default;
        image = default;
        return false;
    }
}
