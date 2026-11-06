using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class BackToMenuBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Back)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        services.Database.Users.ExitRoom(context.User);
        return services.GlobalHandler.ExitState(chat, context.User);
    }
}
