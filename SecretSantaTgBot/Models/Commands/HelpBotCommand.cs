using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.Commands.Descriptions;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands;

public class HelpBotCommand(
    ServicesContext services,
    IEnumerable<IBotCommand> commands)
    : BotCommandBase("/help", BotCommandDescriptions.HelpCommand)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        var isAdmin = context.User.IsAdmin();
        var msg = services.MessageBuilder.BuildHelpMessage(commands, isAdmin);
        return services.Notifications.SendMessage(chat.Id, msg);
    }
}
