using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Extensions;

public static class UserExtensions
{
    extension(UserEntity user)
    {
        public IEnumerable<PartyRoomEntity> GetAdminRooms()
            => user.AvailableRooms.Where(x => x.Admin.Id == user.Id);

        public PartyRoomEntity? GetAvailableRoom(Guid roomId)
            => user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id));
        public PartyRoomEntity? GetAvailableRoom(string roomIdStr)
            => Guid.TryParse(roomIdStr, out var roomId)
                ? user.GetAvailableRoom(roomId)
                : default;

        public ParticipantEntity? GetAsParticipant(PartyRoomEntity room)
            => room.Participants.FirstOrDefault(x => x.Id == user.Id);
        public ParticipantEntity? GetAsParticipant()
            => user.SelectedRoom is { } room
                ? user.GetAsParticipant(room)
                : default;

        public bool IsAdmin(PartyRoomEntity room)
            => room.Admin.Id == user.Id;
        public bool IsAdmin()
            => user.SelectedRoom is { } room && user.IsAdmin(room);
    }
}
