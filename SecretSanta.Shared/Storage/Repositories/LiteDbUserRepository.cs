using LiteDB;

using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Utils;

namespace SecretSanta.Shared.Storage.Repositories;

public sealed class LiteDbUserRepository(LiteDatabase db)
    : LiteDbRepositoryBase<UserEntity>(db, "users"), IUserRepository
{
    private ILiteCollection<UserEntity> FullUsers => Collection
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom)
            .Include(x => x.AvailableRooms.Select(x => x.Admin));

    public Task<UserEntity> GetOrCreateAsync(
        long id,
        string? username,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = FullUsers.FindById(id);

        if (user is null)
        {
            user = new UserEntity()
            {
                TelegramUserId = id,
                Username = username,
                PrefferedLanguage = LanguageUtil.ConvertLanguageToSystem(language),
                AvailableRooms = []
            };

            Collection.Insert(user);
            return Task.FromResult(user);
        }

        if (username is not null && user.Username?.Equals(username) != true)
        {
            user.Username = username;
            Collection.Update(user);
        }

        if (user.PrefferedLanguage is null && language is not null)
        {
            user.PrefferedLanguage = language;
            Collection.Update(user);
        }

        return Task.FromResult(user);
    }
    public Task<IReadOnlyList<UserEntity>> GetByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userIds = ids.ToArray();
        IReadOnlyList<UserEntity> users = [.. FullUsers.Find(x => userIds.Contains(x.TelegramUserId))];
        return Task.FromResult(users);
    }

    public Task<bool> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Collection.Update(user));
    }

    public Task UpdateAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Collection.Update(users);
        return Task.CompletedTask;
    }

    public Task<bool> SetStateAsync(
        UserEntity user,
        StateModel state,
        CancellationToken cancellationToken = default)
    {
        user.CurrentState = state.ToString();
        return UpdateAsync(user, cancellationToken);
    }

    public Task<bool> ClearStateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.CurrentState = default;
        return UpdateAsync(user, cancellationToken);
    }


    public Task<IReadOnlyList<PartyRoomEntity>> GetRoomsAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<PartyRoomEntity> rooms = FullUsers.FindById(telegramUserId)?.AvailableRooms ?? [];
        return Task.FromResult(rooms);
    }

    public Task<bool> EnterRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default)
    {
        user.SelectedRoom = room;
        user.CurrentState = default;
        return UpdateAsync(user, cancellationToken);
    }

    public Task<bool> ExitRoomAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.SelectedRoom = default;
        user.CurrentState = default;
        return UpdateAsync(user, cancellationToken);
    }
}
