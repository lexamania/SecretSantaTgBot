using LiteDB;

using SecretSanta.Shared.Storage.Repositories;
using SecretSanta.Shared.Utils;

namespace SecretSanta.Shared.Storage;

public sealed class LiteDbSantaDatabase : ISantaDatabase
{
    private readonly LiteDatabase _database;

    public IUserRepository Users { get; }
    public IRoomRepository Rooms { get; }

    public LiteDbSantaDatabase(string dbPath)
    {
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _database = new LiteDatabase(new ConnectionString
        {
            Filename = dbPath,
            Connection = ConnectionType.Shared
        });
        Users = new LiteDbUserRepository(_database);
        Rooms = new LiteDbRoomRepository(_database, Users);
    }

    public void Dispose()
    {
        _database.Dispose();
        GC.SuppressFinalize(this);
    }
}
