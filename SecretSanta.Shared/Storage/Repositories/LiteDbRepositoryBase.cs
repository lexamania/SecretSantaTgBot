using LiteDB;

namespace SecretSanta.Shared.Storage.Repositories;

public abstract class LiteDbRepositoryBase<T>(LiteDatabase database, string collectionName)
{
    protected ILiteCollection<T> Collection { get; } = database.GetCollection<T>(collectionName);
}
