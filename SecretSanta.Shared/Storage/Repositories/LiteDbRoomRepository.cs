using LiteDB;
using LiteDB.Async;

using SecretSanta.Shared.Extensions;
using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.Shared.Storage.Repositories;

public sealed class LiteDbRoomRepository(ILiteDatabaseAsync database, IUserRepository users)
    : LiteDbRepositoryBase<PartyRoomEntity>(database, "rooms"), IRoomRepository
{
    public async Task<PartyRoomEntity?> GetByIdAsync(
        RoomId id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Collection.FindByIdAsync(id.Value);
    }

    public async Task<IReadOnlyList<PartyRoomEntity>> GetByParticipantAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return
        [
            .. await Collection.FindAsync(
                Query.Any().EQ("$.Participants[*]._id", telegramUserId))
        ];
    }

    public async Task<IReadOnlyList<PartyRoomEntity>> GetByAdminAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return
        [
            .. await Collection.FindAsync(
                room => room.AdminTelegramUserId == telegramUserId)
        ];
    }

    public Task<PartyRoomEntity> CreateAsync(
        long adminTelegramUserId,
        string title,
        string description,
        string adminInRoomName,
        CancellationToken cancellationToken = default)
        => LiteDbUnitOfWork.ExecuteAsync(
            Database,
            unitOfWork => unitOfWork.Rooms.CreateInUnitOfWorkAsync(
                adminTelegramUserId,
                title,
                description,
                adminInRoomName,
                cancellationToken),
            cancellationToken);

    public async Task UpdateAsync(
        PartyRoomEntity room,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!await Collection.UpdateAsync(room))
        {
            throw new InvalidOperationException("The room record could not be updated.");
        }
    }

    public Task<PartyRoomEntity> DeleteAsync(
        RoomId roomId,
        CancellationToken cancellationToken = default)
        => LiteDbUnitOfWork.ExecuteAsync(
            Database,
            unitOfWork => unitOfWork.Rooms.DeleteInUnitOfWorkAsync(
                roomId,
                cancellationToken),
            cancellationToken);

    public Task AddUserToRoomAsync(
        long userTelegramId,
        RoomId roomId,
        string userInRoomName,
        CancellationToken cancellationToken = default)
        => LiteDbUnitOfWork.ExecuteAsync(
            Database,
            unitOfWork => unitOfWork.Rooms.AddUserToRoomInUnitOfWorkAsync(
                userTelegramId,
                roomId,
                userInRoomName,
                cancellationToken),
            cancellationToken);

    public Task RemoveUserFromRoomAsync(
        long userTelegramId,
        RoomId roomId,
        CancellationToken cancellationToken = default)
        => LiteDbUnitOfWork.ExecuteAsync(
            Database,
            unitOfWork => unitOfWork.Rooms.RemoveUserFromRoomInUnitOfWorkAsync(
                userTelegramId,
                roomId,
                cancellationToken),
            cancellationToken);

    internal async Task<PartyRoomEntity> CreateInUnitOfWorkAsync(
        long adminTelegramUserId,
        string title,
        string description,
        string adminInRoomName,
        CancellationToken cancellationToken)
    {
        var transactionAdmin = await users.GetByIdAsync(adminTelegramUserId, cancellationToken)
            ?? throw new InvalidOperationException("The room administrator was not found.");
        var room = new PartyRoomEntity
        {
            Title = title,
            Description = description,
            AdminTelegramUserId = transactionAdmin.TelegramUserId
        };

        cancellationToken.ThrowIfCancellationRequested();
        await Collection.InsertAsync(room);
        await AddUserToRoomCoreAsync(
            room,
            transactionAdmin,
            adminInRoomName,
            cancellationToken);
        return room;
    }

    internal async Task<PartyRoomEntity> DeleteInUnitOfWorkAsync(
        RoomId roomId,
        CancellationToken cancellationToken)
    {
        var room = await GetByIdAsync(roomId, cancellationToken)
            ?? throw new InvalidOperationException("The room to delete was not found.");

        var participantIds = room.Participants
            .Select(participant => participant.TelegramUserId)
            .ToArray();
        var roomUsers = await users.GetByIdsAsync(participantIds, cancellationToken);

        foreach (var roomUser in roomUsers)
        {
            if (roomUser.SelectedRoom?.Id == room.Id)
            {
                roomUser.SelectedRoom = null;
                roomUser.CurrentState = null;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        await users.UpdateAsync(roomUsers, cancellationToken);

        if (!await Collection.DeleteAsync(room.Id.Value))
        {
            throw new InvalidOperationException("The room could not be deleted.");
        }

        return room;
    }

    internal async Task AddUserToRoomInUnitOfWorkAsync(
        long userTelegramId,
        RoomId roomId,
        string userInRoomName,
        CancellationToken cancellationToken)
    {
        var transactionUser = await users.GetByIdAsync(userTelegramId, cancellationToken)
            ?? throw new InvalidOperationException("The user to add to the room was not found.");
        var transactionRoom = await GetByIdAsync(roomId, cancellationToken)
            ?? throw new InvalidOperationException("The room to join was not found.");

        await AddUserToRoomCoreAsync(
            transactionRoom,
            transactionUser,
            userInRoomName,
            cancellationToken);
    }

    internal async Task RemoveUserFromRoomInUnitOfWorkAsync(
        long userTelegramId,
        RoomId roomId,
        CancellationToken cancellationToken)
    {
        var transactionUser = await users.GetByIdAsync(userTelegramId, cancellationToken)
            ?? throw new InvalidOperationException("The user to remove from the room was not found.");
        var transactionRoom = await GetByIdAsync(roomId, cancellationToken)
            ?? throw new InvalidOperationException("The room to leave was not found.");

        transactionRoom.Participants.RemoveFirst(
            participant => participant.TelegramUserId == transactionUser.TelegramUserId);

        cancellationToken.ThrowIfCancellationRequested();
        if (!await Collection.UpdateAsync(transactionRoom))
        {
            throw new InvalidOperationException("The room membership could not be updated.");
        }

        await users.ExitRoomAsync(transactionUser, cancellationToken);
    }

    private async Task AddUserToRoomCoreAsync(
        PartyRoomEntity room,
        UserEntity user,
        string userInRoomName,
        CancellationToken cancellationToken)
    {
        room.Participants.Add(new ParticipantEntity
        {
            TelegramUserId = user.TelegramUserId,
            Username = user.Username,
            CustomName = userInRoomName
        });

        cancellationToken.ThrowIfCancellationRequested();
        if (!await Collection.UpdateAsync(room))
        {
            throw new InvalidOperationException("The room membership could not be updated.");
        }

        await users.UpdateAsync(user, cancellationToken);
    }
}
