using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;

using Telegram.Bot.Types;
using SecretSantaTgBot.Models.MessageStates.Base;

namespace SecretSantaTgBot.Models.Commands.General;

public class HelpBotCommand(
    ServicesContext services,
    MessageStateBase state)
    : BotCommandBase(BotCommandsList.Help)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var msg = services.MessageBuilder.BuildHelpMessage(
            state.ReadOnlyCommands.Values,
            context.User.IsAdmin());
        return services.Notifications.SendMessage(chat.Id, msg);
    }
}
