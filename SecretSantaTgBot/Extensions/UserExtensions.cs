using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Extensions;

public static class UserExtensions
{
    extension(UserEntity user)
    {
        public IEnumerable<PartyRoomEntity> GetAdminRooms()
            => user.AvailableRooms.Where(x => x.Admin.TelegramUserId == user.TelegramUserId);

        public PartyRoomEntity? GetAvailableRoom(Guid roomId)
            => user.AvailableRooms.FirstOrDefault(x => roomId.Equals(x.Id));
        public PartyRoomEntity? GetAvailableRoom(string roomIdStr)
            => Guid.TryParse(roomIdStr, out var roomId)
                ? user.GetAvailableRoom(roomId)
                : default;

        public ParticipantEntity? GetAsParticipant(PartyRoomEntity room)
            => room.Participants.FirstOrDefault(x => x.TelegramUserId == user.TelegramUserId);
        public ParticipantEntity? GetAsParticipant()
            => user.SelectedRoom is { } room
                ? user.GetAsParticipant(room)
                : default;

        public bool IsAdmin(PartyRoomEntity room)
            => room.Admin.TelegramUserId == user.TelegramUserId;
        public bool IsAdmin()
            => user.SelectedRoom is { } room && user.IsAdmin(room);
    }
}
