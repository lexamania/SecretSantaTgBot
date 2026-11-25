using SecretSanta.TelegramBot.Enums;
using System.Text;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Configurations;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.TelegramBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public class MessageBuilder(BotLocalizer localizer, BotConfiguration config)
{
    public string BuildHelpMenu(IEnumerable<IBotCommand> commands, bool isAdmin)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{localizer.Messages.Menu}</b>");
        strBldr.AppendLine();

        foreach (var command in commands.Where(x => x.Access.HasAccess(isAdmin)))
            strBldr.AppendLine($"{command.Command} - {BotCommandsList.Descriptions.Get(command.Command, localizer)!}");

        return strBldr.ToString();
    }

    public string BuildSantaFinished(PartyRoomEntity room, ParticipantEntity target)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{room.Title}</b>");
        strBldr.AppendLine(localizer.Messages.NtfSantaFinished);
        strBldr.AppendLine();
        strBldr.AppendLine($"{BuildTargetInfo(target)}");
        return strBldr.ToString();
    }

    public string BuildTargetInfo(ParticipantEntity participant)
        => $"{localizer.Messages.UserTarget}\n{BuildUserInfo(participant)}";

    public string BuildUserInfo(ParticipantEntity participant)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{GetUserName(participant)}</b>");

        if (participant.Wishes.Count > 0)
        {
            strBldr.AppendLine();
            strBldr.AppendLine($"<b>{localizer.Messages.WishesList}</b>");

            foreach (var wish in participant.Wishes.Where(x => x.ImageIds is not { Count: > 0 }))
                strBldr.AppendLine($" ► {wish.Message}");            
        }

        return strBldr.ToString();
    }

    public string BuildRoomInfo(PartyRoomEntity room)
    {
        var admin = room.Participants.First(x => x.TelegramUserId == room.Admin.TelegramUserId);
        var participants = room.Participants;

        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>\"{room.Title}\"</b>");
        strBldr.AppendLine();
        strBldr.AppendLine($"{room.Description}");
        strBldr.AppendLine();
        strBldr.AppendLine($"{localizer.Messages.RoomAdmin} {GetUserName(admin)}");
        strBldr.AppendLine($"<b>{localizer.Messages.RoomLink}</b> {GetJoinLink(room.Id)}");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{localizer.Messages.ParticipantsList} ({participants.Count}):</b>");
        strBldr.AppendLine();

        foreach (var prt in participants)
            strBldr.AppendLine($" ► {GetUserName(prt)}");

        return strBldr.ToString();
    }

    public string BuildRoomsList(UserEntity user)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{localizer.Messages.RoomsList}</b>");
        strBldr.AppendLine();

        foreach (var room in user.AvailableRooms)
        {
            var admin = room.Admin.Username is not null
                ? $" - @{room.Admin.Username}"
                : string.Empty;

            strBldr.AppendLine($" ► <b>{room.Title}</b>{admin}");
            strBldr.AppendLine($"{localizer.Messages.RoomLink} {GetJoinLink(room.Id)}");
            strBldr.AppendLine();
        }

        return strBldr.ToString();
    }

    public string BuildRoomCreated(RoomId roomId)
        => $"{localizer.Messages.RoomCreated}\n\n{localizer.Messages.RoomLink} {GetJoinLink(roomId)}";

    public string BuildUserLeft(ParticipantEntity p)
        => $"{GetUserName(p)} {localizer.Messages.NtfUserLeftRoom}";

    public string BuildRoomDeleted(PartyRoomEntity room)
        => $"<b>{room.Title}</b> {localizer.Messages.RoomDeleted}";

    public string BuildNewParticipant(PartyRoomEntity room, ParticipantEntity participant)
        => $"{GetUserName(participant)} {localizer.Messages.NewParticipant} <b>{room.Title}</b>";

    public string BuildErrLeavingRestricted()
        => $"{localizer.Messages.ErrRoomLeaving}\n{localizer.Messages.ErrSantaWasPlayed}";

    public string BuildErrJoiningRestricted()
        => $"{localizer.Messages.ErrRoomJoining}\n{localizer.Messages.ErrSantaWasPlayed}";



    private string GetJoinLink(RoomId roomId)
        => $"https://t.me/{config.BotName}?start={roomId}";

    private string GetUserName(ParticipantEntity p)
        => p.Username is not null
            ? $"{p.CustomName} (@{p.Username})"
            : p.CustomName;
}
