using SecretSanta.Shared.Storage;
using SecretSanta.TelegramApi.Attributes;
using SecretSanta.TelegramApi.Models;

namespace SecretSanta.TelegramApi.Services;

[RequiredService(InjectionType.Singleton)]
public sealed class RoomsService(
    SantaDatabase database)
{
    public ServiceResult<IReadOnlyList<RoomListItem>> GetRooms(TelegramInitData initData)
    {
        var rooms = database.Users.GetRooms(initData.User.Id)
            .Select(room => new RoomListItem(
                room.Id,
                room.Title,
                room.Description,
                room.Participants.Count,
                room.Admin.TelegramUserId == initData.User.Id,
                room.SantaPlayed))
            .ToList();

        return ServiceResult<IReadOnlyList<RoomListItem>>.Success(rooms);
    }
}
