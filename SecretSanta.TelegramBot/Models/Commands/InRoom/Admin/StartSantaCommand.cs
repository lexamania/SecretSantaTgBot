using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.InRoom.Admin;

public class StartSantaCommand(ServicesContext services)
    : BotCommandBase(
        BotCommandsList.StartSanta,
        access: AccessRights.Admin)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var room = context.User.SelectedRoom!;

        if (!context.User.IsAdmin())
        {
            await services.Notifications.SendNeedAdminRights(chat.Id);
            return;
        }

        if (room.SantaPlayed)
        {
            await services.Notifications.SendMessage(
                chat.Id, services.Localizer.Messages.ErrSantaWasPlayed);
            return;
        }

        var participants = room.Participants;
        if (participants.Count < 2)
        {
            await services.Notifications.SendMessage(
                chat.Id, services.Localizer.Messages.ErrSantaNotEnoughParticipants);
            return;
        }

        room.Participants = ShuffleTargets(room.Participants);
        room.SantaPlayed = true;
        await services.Database.Rooms.UpdateAsync(room);

        await NotifyEveryoneTheirTarget(room);
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
