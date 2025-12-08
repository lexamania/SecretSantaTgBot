using System.Text;

using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage.Models;

namespace SecretSantaTgBot.Utils;

public static class MessageBuilder
{
    private static MessagesBase Msgs => EnvVariables.Messages;

    public static string BuildHelpMessage(IEnumerable<CommandInfo> commands, bool isAdmin)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{Msgs.CommandBotMenu}</b>");

        foreach (var command in commands.Where(x => x.ShowHelp 
                                                && (x.Access == AccessRights.Default
                                                    || (x.Access == AccessRights.Admin && isAdmin)
                                                    || (x.Access == AccessRights.NotForAdmin && !isAdmin))))
            strBldr.AppendLine($"{command.Command} - {command.Description}");

        return strBldr.ToString();
    }

    public static string BuildTargetMessage(PartyRoom room, Participant target)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine($"{Msgs.RoomNumber} \"{room.Title}\"");
        strBldr.AppendLine(Msgs.SantaFinished);
        strBldr.AppendLine($"{Msgs.UserTarget} {target.RealName} @{target.Username}");
        return strBldr.ToString();
    }

    public static string BuildUserInfoMessage(string header, Participant participant)
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

    public static string BuildRoomInfoMessage(PartyRoom room)
    {
        var admin = room.Users.First(x => x.Id == room.Admin.Id);
        var participants = room.Users;

        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>\"{room.Title}\"</b>");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>room link: </b>{NameParser.GetRoomJoinLink(room.Id.ToString())}");
        strBldr.AppendLine($"admin: {admin.RealName} - @{admin.Username}");
        strBldr.AppendLine();
        strBldr.AppendLine($"{room.PartyDescription}");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{Msgs.ParticipantsList} ({participants.Count})</b>");

        foreach (var prt in participants)
            strBldr.AppendLine($" ► {prt.RealName} - @{prt.Username}");

        return strBldr.ToString();
    }

    public static string BuildRoomsInfoMessage(UserTg user)
    {
        var strBldr = new StringBuilder();
        strBldr.AppendLine(Msgs.RoomsList);
        strBldr.AppendLine();

        foreach (var room in user.AvailableRooms)
        {
            var adminText = room.Admin.Id == user.Id
                ? " - admin"
                : string.Empty;

            strBldr.AppendLine($" ► <b>{room.Title}{adminText}</b>");
            strBldr.AppendLine($"   {NameParser.GetRoomJoinLink(room.Id.ToString())}");
            strBldr.AppendLine($"   {room.PartyDescription}");
            strBldr.AppendLine();
        }

        return strBldr.ToString();
    }

    public static string BuildLeaveMessage(Participant p)
        => $"{p.RealName} (@{p.Username}) {Msgs.UserLeavedRoomForAll}";

    public static string BuildCreateRoomMessage(string roomId)
        => $"{Msgs.RoomCreated} {roomId}\n{NameParser.GetRoomJoinLink(roomId)}";

    public static string BuildDeleteRoomMessage(PartyRoom room)
        => $"{room.Title} ({room.Id}) {Msgs.RoomDeleted}";
}
