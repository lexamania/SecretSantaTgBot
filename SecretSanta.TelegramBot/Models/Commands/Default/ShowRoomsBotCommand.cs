using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.Default;

public class ShowRoomsBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowRooms)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var rooms = await services.Database.Rooms.GetByParticipantAsync(
            context.User.TelegramUserId);

        if (rooms.Count == 0)
        {
            await services.Notifications.SendMessage(
                chat.Id, services.Localizer.Messages.ErrZeroRooms);
            return;
        }

        var message = services.MessageBuilder.BuildRoomsList(rooms);
        await services.Notifications.SendStagedMessage(chat.Id, message);
    }
}
