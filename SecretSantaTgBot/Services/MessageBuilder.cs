using System.Text;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Localization.Dictionaries;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Services;

[RequiredService(InjectionType.Singleton)]
public class MessageBuilder(
    LocalizationDispatcher msgDspt,
    BotConfiguration config)
{
    // Todo: finish localization
    private readonly ILocalization _msgs = msgDspt.Get("UA");

    public string BuildHelpMessage(IEnumerable<CommandModel> commands, bool isAdmin)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{_msgs.CommandBotMenu}</b>");

        foreach (var command in commands.Where(x => x.ShowHelp 
                                                && (x.Access == AccessRights.Default
                                                    || (x.Access == AccessRights.Admin && isAdmin)
                                                    || (x.Access == AccessRights.NotForAdmin && !isAdmin))))
            strBldr.AppendLine($"{command.Command} - {command.Description}");

        return strBldr.ToString();
    }

    public string BuildTargetMessage(PartyRoomEntity room, ParticipantEntity target)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"{_msgs.RoomNumber} \"{room.Title}\"");
        strBldr.AppendLine(_msgs.SantaFinished);
        strBldr.AppendLine($"{_msgs.UserTarget} {target.RealName} @{target.Username}");
        return strBldr.ToString();
    }

    public string BuildUserInfoMessage(string header, ParticipantEntity participant)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{participant.RealName} - @{participant.Username}</b>");

        if (participant.Wishes.Count > 0)
        {
            strBldr.AppendLine();
            strBldr.AppendLine($"<b>{header}</b>");

            foreach (var wish in participant.Wishes.Where(x => x.Images is not { Count: > 0 }))
                strBldr.AppendLine($" ► {wish.Message}");            
        }

        return strBldr.ToString();
    }

    public string BuildRoomInfoMessage(PartyRoomEntity room)
    {
        var admin = room.Users.First(x => x.Id == room.Admin.Id);
        var participants = room.Users;

        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>\"{room.Title}\"</b>");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>room link: </b>{BuildJoinLink(room.Id.ToString())}");
        strBldr.AppendLine($"admin: {admin.RealName} - @{admin.Username}");
        strBldr.AppendLine();
        strBldr.AppendLine($"{room.PartyDescription}");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{_msgs.ParticipantsList} ({participants.Count})</b>");

        foreach (var prt in participants)
            strBldr.AppendLine($" ► {prt.RealName} - @{prt.Username}");

        return strBldr.ToString();
    }

    public string BuildRoomsInfoMessage(UserEntity user)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{_msgs.RoomsList}</b>");
        strBldr.AppendLine();

        foreach (var room in user.AvailableRooms)
        {
            var adminText = room.Admin.Id == user.Id
                ? " - admin"
                : string.Empty;

            strBldr.AppendLine($" ► <b>{room.Title}{adminText}</b>");
            strBldr.AppendLine($"{room.PartyDescription}");
            strBldr.AppendLine($"{BuildJoinLink(room.Id.ToString())}");
            strBldr.AppendLine();
        }

        return strBldr.ToString();
    }

    public string BuildLeaveMessage(ParticipantEntity p)
        => $"{p.RealName} (@{p.Username}) {_msgs.UserLeavedRoomForAll}";

    public string BuildCreateRoomMessage(string roomId)
        => $"{_msgs.RoomCreated} {roomId}\n{BuildJoinLink(roomId)}";

    public string BuildDeleteRoomMessage(PartyRoomEntity room)
        => $"{room.Title} ({room.Id}) {_msgs.RoomDeleted}";

    public string BuildJoinLink(string roomId)
        => $"t.me/{config.BotName}?start={roomId}";
}
