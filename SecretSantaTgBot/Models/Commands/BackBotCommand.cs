using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Models.Commands.Descriptions;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands;

public class BackBotCommand(ServicesContext services) : BotCommandBase("/back", BotCommandDescriptions.HelpCommand, services)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        throw new NotImplementedException();
    }
}
