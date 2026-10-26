using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.General;

public class BackBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Back)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        services.Database.Users.UpdateWithStateClearing(context.User);
        return context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
