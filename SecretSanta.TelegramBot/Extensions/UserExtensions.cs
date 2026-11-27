using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.TelegramBot.Extensions;

public static class UserExtensions
{
    extension(UserEntity user)
    {
        public ParticipantEntity? GetAsParticipant(PartyRoomEntity room)
            => room.Participants.FirstOrDefault(x => x.TelegramUserId == user.TelegramUserId);
        public ParticipantEntity? GetAsParticipant()
            => user.SelectedRoom is { } room
                ? user.GetAsParticipant(room)
                : default;

        public bool IsAdmin(PartyRoomEntity room)
            => room.AdminTelegramUserId == user.TelegramUserId;
        public bool IsAdmin()
            => user.SelectedRoom is { } room && user.IsAdmin(room);

        public bool IsParticipant(PartyRoomEntity room)
            => room.Participants.Any(p => p.TelegramUserId == user.TelegramUserId);
    }
}
