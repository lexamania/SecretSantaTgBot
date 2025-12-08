using System.Text;

using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;

namespace SecretSantaTgBot.Utils;

public static class MessageBuilder
{
    public static string GetHelpMessage(MessagesBase msgs, IEnumerable<CommandInfo> commands, bool isAdmin)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{msgs.CommandBotMenu}</b>");

        foreach (var command in commands.Where(x => x.ShowHelp 
                                                && (x.Access == AccessRights.Default
                                                    || (x.Access == AccessRights.Admin && isAdmin)
                                                    || (x.Access == AccessRights.NotForAdmin && !isAdmin))))
            strBldr.AppendLine($"{command.Command} - {command.Description}");

        return strBldr.ToString();
    }
}
