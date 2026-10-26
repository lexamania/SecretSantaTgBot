using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class LeaveRoomConfirmationBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.LeaveRoom)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;
        var participant = context.User.GetAsParticipant(room)!;

        services.Database.RemoveUserFromRoom(context.User, room);

        var message = services.MessageBuilder.BuildLeaveMessage(participant);

        return Task.WhenAll(
            services.Notifications.NotifyEveryoneInRoom(room, message),
            services.Notifications.SendMessage(
                chat.Id,
                context.Localization.UserLeavedRoom)
        );
    }
}
