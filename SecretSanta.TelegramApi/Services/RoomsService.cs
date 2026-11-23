using SecretSanta.Shared.Storage;
using SecretSanta.TelegramApi.Attributes;
using SecretSanta.TelegramApi.Models;

namespace SecretSanta.TelegramApi.Services;

[RequiredService(InjectionType.Singleton)]
public sealed class RoomsService(
    ISantaDatabase database)
{
    public async Task<ServiceResult<IReadOnlyList<RoomListItem>>> GetRoomsAsync(
        TelegramInitData initData,
        CancellationToken cancellationToken = default)
    {
        var rooms = (await database.Users.GetRoomsAsync(initData.User.Id, cancellationToken))
            .Select(room => new RoomListItem(
                room.Id.Value,
                room.Title,
                room.Description,
                room.Participants.Count,
                room.Admin.TelegramUserId == initData.User.Id,
                room.SantaPlayed))
            .ToList();

        return ServiceResult<IReadOnlyList<RoomListItem>>.Success(rooms);
    }
}
