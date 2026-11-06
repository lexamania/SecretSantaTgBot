using SecretSanta.TelegramBot.Enums;
using System.Text;

using Microsoft.Extensions.Localization;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Configurations;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.TelegramBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public class MessageBuilder(
    IStringLocalizer<BotContent> texts,
    BotConfiguration config)
{
    public string GetByKey(string textKey)
        => texts[textKey];

    public string BuildHelpMenu(IEnumerable<IBotCommand> commands, bool isAdmin)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{texts[BotContent.Messages.Menu]}</b>");
        strBldr.AppendLine();

        foreach (var command in commands.Where(x => x.Access.HasAccess(isAdmin)))
            strBldr.AppendLine($"{command.Command} - {texts[BotCommandsList.Descriptions.GetKey(command.Command)!]}");

        return strBldr.ToString();
    }

    public string BuildSantaFinished(PartyRoomEntity room, ParticipantEntity target)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{room.Title}</b>");
        strBldr.AppendLine(texts[BotContent.Messages.NtfSantaFinished]);
        strBldr.AppendLine();
        strBldr.AppendLine($"{BuildTargetInfo(target)}");
        return strBldr.ToString();
    }

    public string BuildTargetInfo(ParticipantEntity participant)
        => $"{texts[BotContent.Messages.UserTarget]}\n{BuildUserInfo(participant)}";

    public string BuildUserInfo(ParticipantEntity participant)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{GetUserName(participant)}</b>");

        if (participant.Wishes.Count > 0)
        {
            strBldr.AppendLine();
            strBldr.AppendLine($"<b>{texts[BotContent.Messages.WishesList]}</b>");

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
        strBldr.AppendLine($"{texts[BotContent.Messages.RoomAdmin]} {GetUserName(admin)}");
        strBldr.AppendLine($"<b>{texts[BotContent.Messages.RoomLink]}</b> {GetJoinLink(room.Id.ToString())}");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{texts[BotContent.Messages.ParticipantsList]} ({participants.Count}):</b>");
        strBldr.AppendLine();

        foreach (var prt in participants)
            strBldr.AppendLine($" ► {GetUserName(prt)}");

        return strBldr.ToString();
    }

    public string BuildRoomsList(UserEntity user)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{texts[BotContent.Messages.RoomsList]}</b>");
        strBldr.AppendLine();

        foreach (var room in user.AvailableRooms)
        {
            var admin = room.Admin.Username is not null
                ? $" - @{room.Admin.Username}"
                : string.Empty;

            strBldr.AppendLine($" ► <b>{room.Title}</b>{admin}");
            strBldr.AppendLine($"{texts[BotContent.Messages.RoomLink]} {GetJoinLink(room.Id.ToString())}");
            strBldr.AppendLine();
        }

        return strBldr.ToString();
    }

    public string BuildRoomCreated(string roomId)
        => $"{texts[BotContent.Messages.RoomCreated]}\n\n{texts[BotContent.Messages.RoomLink]} {GetJoinLink(roomId)}";

    public string BuildUserLeft(ParticipantEntity p)
        => $"{GetUserName(p)} {texts[BotContent.Messages.NtfUserLeftRoom]}";

    public string BuildRoomDeleted(PartyRoomEntity room)
        => $"<b>{room.Title}</b> {texts[BotContent.Messages.RoomDeleted]}";

    public string BuildNewParticipant(PartyRoomEntity room, ParticipantEntity participant)
        => $"{GetUserName(participant)} {texts[BotContent.Messages.NewParticipant]} <b>{room.Title}</b>";

    public string BuildErrLeavingRestricted()
        => $"{texts[BotContent.Messages.ErrRoomLeaving]}\n{texts[BotContent.Messages.ErrSantaWasPlayed]}";

    public string BuildErrJoiningRestricted()
        => $"{texts[BotContent.Messages.ErrRoomJoining]}\n{texts[BotContent.Messages.ErrSantaWasPlayed]}";



    private string GetJoinLink(string roomId)
        => $"https://t.me/{config.BotName}?start={roomId}";

    private string GetUserName(ParticipantEntity p)
        => p.Username is not null
            ? $"{p.CustomName} (@{p.Username})"
            : p.CustomName;
}
