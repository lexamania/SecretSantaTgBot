using SecretSantaTgBot.Services.MessageStates.Base;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot.Utils;

public static class MessageParser
{
    public static bool IsCommand(Message msg, out string? command, out string[]? args)
    {
        if (msg.Type == MessageType.Text && msg.Text!.StartsWith('/'))
        {
            var words = NameParser.ParseArgs(msg.Text!.Trim());
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

    public static bool IsMessage(Message msg, out string? message)
    {
        if (msg.Type == MessageType.Text && !msg.Text!.StartsWith('/'))
        {
            message = msg.Text!.Trim();
            return true;
        }

        message = default;
        return false;
    }

    public static bool IsImage(Message msg, out string? message, out PhotoSize? image)
    {
        if (msg.Type == MessageType.Photo)
        {
            message = msg.Caption!.Trim();
            image = msg.Photo!.First();
            return true;
        }

        message = default;
        image = default;
        return false;
    }

    public static bool HasNewState(Dictionary<string, MessageStateBase> innerStates, string fullState, string currentState, out MessageStateBase? result)
    {
        result = default;

        var states = NameParser.ParseStateArgs(fullState, currentState);
        return states.Length != 0 && innerStates.TryGetValue(states[0], out result);
    }
}
