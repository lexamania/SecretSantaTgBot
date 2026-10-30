using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom;

public class ShowMyTargetBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowMyTarget)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var me = context.User.GetAsParticipant()!;
        if (me.TargetUserId is null)
        {
            await services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.SecretSantaStillOffline);
            return;
        }

        var room = context.User.SelectedRoom!;
        var target = room.Participants.First(x => x.TelegramUserId == me.TargetUserId.Value);
            
        var message = services.MessageBuilder.BuildUserInfoMessage(
            context.Localization.TargetWishesList,
            target);

        await services.Notifications.SendMessage(chat.Id, message);

        foreach (var wish in target.Wishes.Where(x => x.ImageIds is { Count: > 0 }))
            await services.Notifications.SendImages(chat.Id, wish.ImageIds, wish.Message);
    }
}
