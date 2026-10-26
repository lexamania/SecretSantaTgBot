using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class BackToMenuBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Back)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        services.Database.Users.UpdateWithRoomClearing(context.User);
        return context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
}
