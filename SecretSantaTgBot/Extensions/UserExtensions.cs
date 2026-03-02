using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Extensions;

public static class UserExtensions
{
    public static ParticipantEntity? GetAsParticipant(this UserEntity user, PartyRoomEntity room)
        => room.Users.FirstOrDefault(x => x.Id == user.Id);

    public static ParticipantEntity? GetAsParticipant(this UserEntity user)
        => user.SelectedRoom?.Users.FirstOrDefault(x => x.Id == user.Id);

    public static bool IsAdmin(this UserEntity user)
        => user.SelectedRoom?.Admin == user;

    public static bool IsAdmin(this UserEntity user, PartyRoomEntity room)
        => room.Admin == user;
}
