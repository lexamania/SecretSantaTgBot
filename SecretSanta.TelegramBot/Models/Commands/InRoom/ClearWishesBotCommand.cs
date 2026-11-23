using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class ClearWishesBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ClearWishes)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        participant.Wishes.Clear();
        await services.Database.Rooms.UpdateAsync(room);

        await services.Notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.UserWishesIsCleared);
    }
}
