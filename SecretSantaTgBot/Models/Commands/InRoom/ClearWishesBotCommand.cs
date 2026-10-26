using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class ClearWishesBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ClearWishes)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        participant.Wishes.Clear();
        services.Database.Rooms.Update(room);

        return services.Notifications.SendMessage(
            chat.Id,
            context.Localization.UserWishesCleared);
    }
}
