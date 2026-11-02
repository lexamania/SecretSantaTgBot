using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom.Admin;

public class StartSantaCommand(ServicesContext services)
    : BotCommandBase(
        BotCommandsList.StartSanta,
        access: AccessRights.Admin)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;

        if (!context.User.IsAdmin())
            return services.Notifications.SendNeedAdminRights(chat.Id);

        if (room.SantaPlayed)
        {
            return services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrSantaWasPlayed);
        }

        var participants = room.Participants;
        if (participants.Count < 2)
        {
            return services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrSantaNotEnoughParticipants);
        }

        room.Participants = ShuffleTargets(room.Participants);
        room.SantaPlayed = true;
        services.Database.Rooms.Update(room);

        return NotifyEveryoneTheirTarget(room);
    }

    private List<ParticipantEntity> ShuffleTargets(List<ParticipantEntity> participants)
    {
        var pArray = participants.ToArray();
        var targetListIds = RandomUtility.GetShuffledUniqueIndexRange(pArray.Length);

        for (int i = 0; i < pArray.Length; ++i)
            pArray[i].TargetUserId = pArray[targetListIds[i]].TelegramUserId;

        return participants;
    }

    private Task NotifyEveryoneTheirTarget(PartyRoomEntity room)
    {
        var tasks = new List<Task>();

        foreach (var p in room.Participants)
        {
            var target = room.Participants.First(x => x.TelegramUserId == p.TargetUserId);
            var message = services.MessageBuilder.BuildSantaFinished(room, target);
            tasks.Add(services.Notifications.SendStagedMessage(p.TelegramUserId, message));
        }

        return Task.WhenAll(tasks);
    }
}
