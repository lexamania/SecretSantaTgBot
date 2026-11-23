using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class BackToMenuBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Back)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        await services.Database.Users.ExitRoomAsync(context.User);
        await services.GlobalHandler.ExitState(chat, context.User);
    }
}
