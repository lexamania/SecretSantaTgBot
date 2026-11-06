using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class ClearWishesBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ClearWishes)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        participant.Wishes.Clear();
        services.Database.Rooms.Update(room);

        return services.Notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.UserWishesIsCleared);
    }
}
