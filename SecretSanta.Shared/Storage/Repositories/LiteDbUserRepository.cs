using LiteDB.Async;

using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Localization.Services;

namespace SecretSanta.Shared.Storage.Repositories;

public sealed class LiteDbUserRepository(ILiteDatabaseAsync database)
    : LiteDbRepositoryBase<UserEntity>(database, "users"), IUserRepository
{
    private ILiteCollectionAsync<UserEntity> UsersWithSelectedRoom
        => Collection.Include(user => user.SelectedRoom);

    public async Task<UserEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await UsersWithSelectedRoom.FindByIdAsync(id);
    }

    public Task<UserEntity> GetOrCreateAsync(
        long id,
        string? username,
        string? language = null,
        CancellationToken cancellationToken = default)
        => LiteDbUnitOfWork.ExecuteAsync(
            Database,
            unitOfWork => unitOfWork.Users.GetOrCreateInUnitOfWorkAsync(
                id,
                username,
                language,
                cancellationToken),
            cancellationToken);

    internal async Task<UserEntity> GetOrCreateInUnitOfWorkAsync(
        long id,
        string? username,
        string? language,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await UsersWithSelectedRoom.FindByIdAsync(id);

        if (user is null)
        {
            user = new UserEntity
            {
                TelegramUserId = id,
                Username = username,
                PrefferedLanguage = LanguageManager.ConvertLanguageToSystem(language)
            };

            await Collection.InsertAsync(user);
            return user;
        }

        var changed = false;
        if (username is not null && user.Username != username)
        {
            user.Username = username;
            changed = true;
        }

        if (user.PrefferedLanguage is null && language is not null)
        {
            user.PrefferedLanguage = language;
            changed = true;
        }

        if (changed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await Collection.UpdateAsync(user))
            {
                throw new InvalidOperationException("The user profile could not be updated.");
            }
        }

        return user;
    }

    public async Task<IReadOnlyList<UserEntity>> GetByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userIds = ids.ToArray();
        var users = await UsersWithSelectedRoom.FindAsync(
            user => userIds.Contains(user.TelegramUserId));
        return [.. users];
    }

    public async Task UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!await Collection.UpdateAsync(user))
        {
            throw new InvalidOperationException("The user record could not be updated.");
        }
    }

    public async Task UpdateAsync(
        IEnumerable<UserEntity> users,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var usersToUpdate = users as UserEntity[] ?? [.. users];
        var updatedCount = await Collection.UpdateAsync(usersToUpdate);
        if (updatedCount != usersToUpdate.Length)
        {
            throw new InvalidOperationException("Not all affected users could be updated.");
        }
    }

    public Task SetStateAsync(
        UserEntity user,
        StateModel state,
        CancellationToken cancellationToken = default)
    {
        user.CurrentState = state.ToString();
        return UpdateAsync(user, cancellationToken);
    }

    public Task ClearStateAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.CurrentState = null;
        return UpdateAsync(user, cancellationToken);
    }

    public Task EnterRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default)
    {
        user.SelectedRoom = room;
        user.CurrentState = null;
        return UpdateAsync(user, cancellationToken);
    }

    public Task ExitRoomAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.SelectedRoom = null;
        user.CurrentState = null;
        return UpdateAsync(user, cancellationToken);
    }
}
