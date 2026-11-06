using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom;

public class ShowMyTargetBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.ShowMyTarget)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var me = context.User.GetAsParticipant()!;

        if (me.TargetUserId is null)
        {
            await services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrSantaIsStillOffline);
            return;
        }

        var room = context.User.SelectedRoom!;
        var target = room.Participants.First(x => x.TelegramUserId == me.TargetUserId.Value);
            
        var message = services.MessageBuilder.BuildTargetInfo(target);
        await services.Notifications.SendMessage(chat.Id, message);

        foreach (var wish in target.Wishes.Where(x => x.ImageIds is { Count: > 0 }))
            await services.Notifications.SendImages(chat.Id, wish.ImageIds, wish.Message);
    }
}
