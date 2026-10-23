using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.General;

public class HelpBotCommand(
    ServicesContext services,
    IEnumerable<IBotCommand> commands)
    : BotCommandBase(BotCommandsList.Help)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        var isAdmin = context.User.IsAdmin();
        var msg = services.MessageBuilder.BuildHelpMessage(commands, isAdmin);
        return services.Notifications.SendMessage(chat.Id, msg);
    }
}
