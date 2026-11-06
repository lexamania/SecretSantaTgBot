using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class ShowMeBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowMe)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var participant = context.User.GetAsParticipant()!;

        var message = services.MessageBuilder.BuildUserInfo(participant);
        await services.Notifications.SendMessage(chat.Id, message);

        foreach (var wish in participant.Wishes.Where(x => x.ImageIds is { Count: > 0 }))
            await services.Notifications.SendImages(chat.Id, wish.ImageIds, wish.Message);
    }
}
