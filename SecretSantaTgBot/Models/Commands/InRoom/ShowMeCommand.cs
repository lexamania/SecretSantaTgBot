using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class ShowMeCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRoomInfo)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {

    }
}
