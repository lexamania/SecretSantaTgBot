using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.Shared.Storage.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// Finds a user by their Telegram user ID.
    /// </summary>
    /// <param name="id">The Telegram user ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The user if found; otherwise <see langword="null"/>.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<UserEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user or creates one, updating the username and initially missing language when provided.
    /// </summary>
    /// <param name="id">The Telegram user ID.</param>
    /// <param name="username">The current Telegram username, if available.</param>
    /// <param name="language">The user's Telegram language code, if available.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The existing or newly created user.</returns>
    /// <exception cref="InvalidOperationException">An existing user could not be updated or the transaction could not be committed.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled; the transaction is rolled back.</exception>
    /// <exception cref="AggregateException">The operation failed and the transaction could not be rolled back.</exception>
    Task<UserEntity> GetOrCreateAsync(
        long id,
        string? username,
        string? language = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds users whose Telegram IDs are in the supplied sequence.
    /// </summary>
    /// <param name="ids">Telegram user IDs to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching users; missing IDs are omitted and an empty sequence returns an empty list.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<IReadOnlyList<UserEntity>> GetByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user document.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the user has been updated.</returns>
    /// <exception cref="InvalidOperationException">The user document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple user documents.
    /// </summary>
    /// <param name="users">The users to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all supplied users have been updated.</returns>
    /// <exception cref="InvalidOperationException">The number of updated documents differs from the number supplied.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task UpdateAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the user's current state and persists it.
    /// </summary>
    /// <param name="user">The user whose state will be set.</param>
    /// <param name="state">The state to persist.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the state has been saved.</returns>
    /// <exception cref="InvalidOperationException">The user document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task SetStateAsync(
        UserEntity user,
        StateModel state,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the user's current state and persists the change.
    /// </summary>
    /// <param name="user">The user whose state will be cleared.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the state has been cleared.</returns>
    /// <exception cref="InvalidOperationException">The user document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task ClearStateAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the rooms available to a user.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The user's available rooms, or an empty list if the user does not exist.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<IReadOnlyList<PartyRoomEntity>> GetRoomsAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a user's selected room and clears their current state.
    /// </summary>
    /// <param name="user">The user entering the room.</param>
    /// <param name="room">The room to select.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the room has been selected.</returns>
    /// <exception cref="InvalidOperationException">The user document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task EnterRoomAsync(
        UserEntity user,
        PartyRoomEntity room,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears a user's selected room and current state.
    /// </summary>
    /// <param name="user">The user leaving the room.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the selected room has been cleared.</returns>
    /// <exception cref="InvalidOperationException">The user document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task ExitRoomAsync(UserEntity user, CancellationToken cancellationToken = default);
}
