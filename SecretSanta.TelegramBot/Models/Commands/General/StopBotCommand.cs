using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.General;

public class StopBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Stop)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        await services.Database.Users.ClearStateAsync(context.User);
        await services.GlobalHandler.ExitState(chat, context.User);
    }
}
