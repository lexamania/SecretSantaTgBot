using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class ShowMeBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowMe)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var target = context.User.GetAsParticipant()!;

        var message = services.MessageBuilder.BuildUserInfoMessage(
            context.Localization.UserWishesList,
            target);

        await services.Notifications.SendMessage(chat.Id, message);

        foreach (var wish in target.Wishes.Where(x => x.ImageIds is { Count: > 0 }))
            await services.Notifications.SendImages(chat.Id, wish.ImageIds, wish.Message);
    }
}
