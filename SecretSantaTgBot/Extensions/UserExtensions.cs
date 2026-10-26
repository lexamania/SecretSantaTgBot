using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Extensions;

public static class UserExtensions
{
    public static PartyRoomEntity? GetAvailableRoom(this UserEntity user, Guid roomId)
        => user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id));
    public static PartyRoomEntity? GetAvailableRoom(this UserEntity user, string roomId)
        => user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id.ToString()));

    public static IEnumerable<PartyRoomEntity> GetAdminRooms(this UserEntity user)
        => user.AvailableRooms.Where(x => x.Admin.Id == user.Id);

    public static ParticipantEntity? GetAsParticipant(this UserEntity user, PartyRoomEntity room)
        => room.Participants.FirstOrDefault(x => x.Id == user.Id);
    public static ParticipantEntity? GetAsParticipant(this UserEntity user)
        => user.SelectedRoom?.Participants.FirstOrDefault(x => x.Id == user.Id);


    public static bool IsAdmin(this UserEntity user)
        => user.SelectedRoom?.Admin.Id == user.Id;
    public static bool IsAdmin(this UserEntity user, PartyRoomEntity room)
        => room.Admin.Id == user.Id;
}
