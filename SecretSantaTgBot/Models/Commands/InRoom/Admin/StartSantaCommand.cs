using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.InRoom.Admin;

public class StartSantaCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.StartSanta)
{
    public override Task Execute(Chat chat, BotCommandContext context)
    {
        var room = context.User.SelectedRoom!;

        if (!context.User.IsAdmin())
            return services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.NeedAdminRights);

        if (room.IsPlayed)
            return services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.SecretSantaWasPlayed);

        var participants = room.Participants;
        if (participants.Count < 2)
            return services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.NotEnoughParticipants);

        room.Participants = ShuffleTargets(room.Participants);
        room.IsPlayed = true;
        services.Database.Rooms.Update(room);

        return NotifyEveryoneTheirTarget(room);
    }

    private List<ParticipantEntity> ShuffleTargets(List<ParticipantEntity> participants)
    {
        var pArray = participants.ToArray();
        var targetListIds = RandomUtility.GetShuffledUniqueIndexRange(pArray.Length);

        for (int i = 0; i < pArray.Length; ++i)
            pArray[i].TargetUserId = pArray[targetListIds[i]].Id;

        return participants;
    }

    private Task NotifyEveryoneTheirTarget(PartyRoomEntity room)
    {
        var result = new List<(long Id, string Msg)>();

        foreach (var p in room.Participants)
        {
            var target = room.Participants.First(x => x.Id == p.TargetUserId);
            var message = services.MessageBuilder.BuildTargetMessage(room, target);
            result.Add((p.Id, message));
        }

        return services.Notifications.NotifyEveryone(result);
    }
}
