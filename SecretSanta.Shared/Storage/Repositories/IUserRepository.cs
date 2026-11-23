using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.Shared.Storage.Repositories;

public interface IUserRepository
{
    Task<UserEntity> GetOrCreateAsync(
        long id,
        string? username,
        string? language = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserEntity>> GetByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken = default);

    Task<bool> SetStateAsync(
        UserEntity user,
        StateModel state,
        CancellationToken cancellationToken = default);

    Task<bool> ClearStateAsync(UserEntity user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartyRoomEntity>> GetRoomsAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    Task<bool> EnterRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default);

    Task<bool> ExitRoomAsync(UserEntity user, CancellationToken cancellationToken = default);
}
