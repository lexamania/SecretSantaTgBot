using LiteDB;

namespace SecretSantaTgBot.Storage.Repositories;

public class DirectoryBase<T>(LiteDatabase database, string collectionName)
{
    protected ILiteCollection<T> Collection { get; } = database.GetCollection<T>(collectionName);
}
