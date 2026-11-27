using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Models;

namespace SecretSanta.Shared.Storage.Repositories;

public interface IRoomRepository
{
    /// <summary>
    /// Finds a room by its ID.
    /// </summary>
    /// <param name="id">The room ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The room if it exists; otherwise <see langword="null"/>.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<PartyRoomEntity?> GetByIdAsync(RoomId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rooms in which user is a participant.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The rooms containing the user as a participant.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<IReadOnlyList<PartyRoomEntity>> GetByParticipantAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rooms in which user is an admin.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The rooms containing the user as an admin.</returns>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task<IReadOnlyList<PartyRoomEntity>> GetByAdminAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a room and adds its administrator as the first participant.
    /// </summary>
    /// <param name="adminTelegramUserId">Telegram ID of the user who will administer the room.</param>
    /// <param name="title">The room title.</param>
    /// <param name="description">The room description.</param>
    /// <param name="adminInRoomName">The administrator's display name in the room.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The created room.</returns>
    /// <exception cref="InvalidOperationException">The administrator does not exist or the transaction could not be committed.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled; the transaction is rolled back.</exception>
    /// <exception cref="AggregateException">The operation failed and the transaction could not be rolled back.</exception>
    Task<PartyRoomEntity> CreateAsync(
        long adminTelegramUserId,
        string title,
        string description,
        string adminInRoomName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a room.
    /// </summary>
    /// <param name="room">The room to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the room has been updated.</returns>
    /// <exception cref="InvalidOperationException">The room document could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    Task UpdateAsync(PartyRoomEntity room, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a room and removes it from all participants.
    /// </summary>
    /// <param name="roomId">The ID of the room to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The deleted room, including its participants.</returns>
    /// <exception cref="InvalidOperationException">The room does not exist or a database write fails.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled; the transaction is rolled back.</exception>
    /// <exception cref="AggregateException">The operation failed and the transaction could not be rolled back.</exception>
    Task<PartyRoomEntity> DeleteAsync(
        RoomId roomId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a participant to a room.
    /// </summary>
    /// <param name="userTelegramId">Telegram ID of the user to add.</param>
    /// <param name="roomId">ID of the room to join.</param>
    /// <param name="userInRoomName">The participant's display name in the room.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the membership has been saved.</returns>
    /// <exception cref="InvalidOperationException">The user or room does not exist, or either record could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled; the transaction is rolled back.</exception>
    /// <exception cref="AggregateException">The operation failed and the transaction could not be rolled back.</exception>
    Task AddUserToRoomAsync(
        long userTelegramId,
        RoomId roomId,
        string userInRoomName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a participant from a room.
    /// </summary>
    /// <param name="userTelegramId">Telegram ID of the user to remove.</param>
    /// <param name="roomId">ID of the room to leave.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when membership has been removed.</returns>
    /// <exception cref="InvalidOperationException">The user or room does not exist, or either record could not be updated.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled; the transaction is rolled back.</exception>
    /// <exception cref="AggregateException">The operation failed and the transaction could not be rolled back.</exception>
    Task RemoveUserFromRoomAsync(
        long userTelegramId,
        RoomId roomId,
        CancellationToken cancellationToken = default);
}
