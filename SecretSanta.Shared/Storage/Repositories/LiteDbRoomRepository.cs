using LiteDB;

using SecretSanta.Shared.Extensions;
using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.Shared.Storage.Repositories;

public sealed class LiteDbRoomRepository(LiteDatabase db, IUserRepository users)
    : LiteDbRepositoryBase<PartyRoomEntity>(db, "rooms"), IRoomRepository
{
    public Task<PartyRoomEntity?> GetByIdAsync(RoomId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var room = Collection.Include(x => x.Admin).FindById(id.Value);
        return Task.FromResult<PartyRoomEntity?>(room);
    }

    public async Task<PartyRoomEntity> CreateAsync(
        UserEntity admin,
        string title,
        string description,
        string adminInRoomName,
        CancellationToken cancellationToken = default)
    {
        var room = new PartyRoomEntity
        {
            Title = title,
            Description = description,
            Admin = admin,
        };

        cancellationToken.ThrowIfCancellationRequested();

        Collection.Insert(room);
        await AddUserToRoomAsync(admin, room, adminInRoomName);

        return room;
    }

    public Task<bool> UpdateAsync(PartyRoomEntity room, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Collection.Update(room));
    }

    public async Task<PartyRoomEntity> DeleteAsync(
        UserEntity user,
        RoomId roomId,
        CancellationToken cancellationToken = default)
    {
        var room = user.AvailableRooms.First(r => roomId.Equals(r.Id));
        var userIds = room.Participants.Select(p => p.TelegramUserId).ToList();
        var roomUsers = await users.GetByIdsAsync(userIds, cancellationToken);

        foreach (var roomUser in roomUsers)
        {
            roomUser.AvailableRooms.RemoveFirst(r => r.Id == room.Id);

            if (roomUser.SelectedRoom?.Id == room.Id)
            {
                roomUser.SelectedRoom = default;
                roomUser.CurrentState = default;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        await users.UpdateAsync(roomUsers);
        Collection.Delete(room.Id.Value);

        return room;
    }

    public async Task AddUserToRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        string userInRoomName,
        CancellationToken cancellationToken = default)
    {

        user.AvailableRooms.Add(room);
        room.Participants.Add(new()
        {
            TelegramUserId = user.TelegramUserId,
            Username = user.Username,
            CustomName = userInRoomName
        });

        cancellationToken.ThrowIfCancellationRequested();

        await Task.WhenAll(
            UpdateAsync(room),
            users.UpdateAsync(user));
    }

    public async Task RemoveUserFromRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default)
    {
        user.AvailableRooms.RemoveFirst(x => x.Id == room.Id);
        room.Participants.RemoveFirst(x => x.TelegramUserId == user.TelegramUserId);

        cancellationToken.ThrowIfCancellationRequested();

        await Task.WhenAll(
            UpdateAsync(room),
            users.ExitRoomAsync(user));
    }
}
