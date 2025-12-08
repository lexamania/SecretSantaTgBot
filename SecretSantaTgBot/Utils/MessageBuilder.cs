using System.Text;

using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;

namespace SecretSantaTgBot.Utils;

public static class MessageBuilder
{
    public static string GetHelpMessage(IEnumerable<CommandInfo> commands, MessagesBase msgs)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{msgs.CommandBotMenu}</b>");

        foreach (var command in commands.Where(x => x.ShowHelp))
            strBldr.AppendLine($"{command.Command} - {command.Description}");

        return strBldr.ToString();
    }
}
