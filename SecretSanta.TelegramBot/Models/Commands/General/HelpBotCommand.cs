using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Enums;

using Telegram.Bot.Types;
using SecretSanta.TelegramBot.Models.MessageStates.Base;

namespace SecretSanta.TelegramBot.Models.Commands.General;

public class HelpBotCommand(
    ServicesContext services,
    IMessageState state)
    : BotCommandBase(BotCommandsList.Help)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var msg = services.MessageBuilder.BuildHelpMenu(
            state.ReadOnlyCommands.Values,
            context.User.IsAdmin());
        return services.Notifications.SendMessage(chat.Id, msg);
    }
}
