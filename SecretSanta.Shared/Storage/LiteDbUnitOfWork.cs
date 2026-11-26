using LiteDB.Async;

using SecretSanta.Shared.Storage.Repositories;

namespace SecretSanta.Shared.Storage;

internal sealed class LiteDbUnitOfWork
{
    private LiteDbUnitOfWork(ILiteDatabaseAsync database)
    {
        var users = new LiteDbUserRepository(database);

        Users = users;
        Rooms = new LiteDbRoomRepository(database, users);
    }

    public LiteDbUserRepository Users { get; }
    public LiteDbRoomRepository Rooms { get; }

    public static Task ExecuteAsync(
        ILiteDatabaseAsync database,
        Func<LiteDbUnitOfWork, Task> operation,
        CancellationToken cancellationToken)
        => ExecuteAsync(
            database,
            async unitOfWork =>
            {
                await operation(unitOfWork);
                return true;
            },
            cancellationToken);

    public static async Task<TResult> ExecuteAsync<TResult>(
        ILiteDatabaseAsync database,
        Func<LiteDbUnitOfWork, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var transaction = await database.BeginTransactionAsync();
        var unitOfWork = new LiteDbUnitOfWork(transaction);

        try
        {
            var result = await operation(unitOfWork);
            cancellationToken.ThrowIfCancellationRequested();

            if (!await transaction.CommitAsync())
            {
                throw new InvalidOperationException("The LiteDB transaction could not be committed.");
            }

            return result;
        }
        catch (Exception transactionException)
        {
            if (!await transaction.RollbackAsync())
            {
                throw new AggregateException(
                    "The LiteDB transaction failed and could not be rolled back.",
                    transactionException,
                    new InvalidOperationException("The LiteDB transaction could not be rolled back."));
            }

            throw;
        }
    }
}
