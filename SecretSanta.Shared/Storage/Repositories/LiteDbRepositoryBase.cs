using LiteDB.Async;

namespace SecretSanta.Shared.Storage.Repositories;

public abstract class LiteDbRepositoryBase<T>
{
    protected LiteDbRepositoryBase(ILiteDatabaseAsync database, string collectionName)
    {
        Database = database;
        Collection = database.GetCollection<T>(collectionName);
    }

    protected ILiteDatabaseAsync Database { get; }
    protected ILiteCollectionAsync<T> Collection { get; }
}
