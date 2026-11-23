using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Models;

namespace SecretSanta.Shared.Storage.Repositories;

public interface IRoomRepository
{
    Task<PartyRoomEntity?> GetByIdAsync(RoomId id, CancellationToken cancellationToken = default);

    Task<PartyRoomEntity> CreateAsync(
        UserEntity admin,
        string title,
        string description,
        string adminInRoomName,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PartyRoomEntity room, CancellationToken cancellationToken = default);

    Task<PartyRoomEntity> DeleteAsync(
        UserEntity user,
        RoomId roomId,
        CancellationToken cancellationToken = default);

    Task AddUserToRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        string userInRoomName,
        CancellationToken cancellationToken = default);

    Task RemoveUserFromRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default);
}
