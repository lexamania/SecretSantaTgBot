using System.Text;

using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Utils;

public static class StateUtil
{
    private const char Separetor = '/';
    private const char ArgsEnter = '?';
    private const char ArgsSeparator = '&';

    public static string JoinStates(params string[] states)
        => string.Join(Separetor, states);

    public static string JoinStateArgs(Dictionary<string, string> args)
        => string.Join(ArgsSeparator, args.Select(x => $"{x.Key}={x.Value}"));

    public static string Join(string state, string args)
        => $"{state}{ArgsEnter}{args}";

    public static string Join(IEnumerable<string> states, Dictionary<string, string> args)
    {
        var builder = new StringBuilder();
        builder.AppendJoin(Separetor, states);

        if (args.Count > 0)
        {
            builder.Append(ArgsEnter);
            builder.AppendJoin(ArgsSeparator, args.Select(x => $"{x.Key}={x.Value}"));
        }

        return builder.ToString();
    }



    public static string[] ParseStates(string state)
        => state
            .Split(ArgsEnter).First()
            .Split(Separetor);

    public static Dictionary<string, string> ParseStateArgs(string state)
        => state.Contains(ArgsEnter)
            ? state.Split(ArgsEnter).Last()
                .Split(ArgsSeparator)
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1])
            : [];

    public static (List<string> states, Dictionary<string, string> stateArgs) Parse(string state)
    {
        var splitted = state.Split(ArgsEnter);
        var states = splitted.First().Split(Separetor).ToList();
        var args = splitted.Length >= 2
            ? splitted.Last()
                .Split(ArgsSeparator)
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1])
            : [];

        return (states, args);
    }


    public static Func<Chat, MessageContext, Task> NextStateCallback(MessageStateBase nextState)
        => (chat, context) => nextState.PrepareState(context);
}
