using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.General;

public class StopBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Stop)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        services.Database.Users.ClearState(context.User);
        return context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
