using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.General;

public class BackBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Back)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        services.Database.Users.UpdateWithClearState(context.User);
        return context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
