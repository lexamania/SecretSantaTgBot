namespace SecretSanta.TelegramApi.Models;

public sealed record RoomListItem(
    Guid Id,
    string Title,
    string Description,
    int ParticipantCount,
    bool IsAdmin,
    bool DrawStarted);
