using LiteDB;
using LiteDB.Async;

using SecretSanta.Shared.Storage.Repositories;
using SecretSanta.Shared.Utils;

namespace SecretSanta.Shared.Storage;

public sealed class LiteDbSantaDatabase : ISantaDatabase
{
    private readonly LiteDatabaseAsync _database;

    public IUserRepository Users { get; }
    public IRoomRepository Rooms { get; }

    public LiteDbSantaDatabase(string dbPath)
    {
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _database = new LiteDatabaseAsync(new ConnectionString
        {
            Filename = dbPath,
            // LiteDB.Async recommends Direct connections for its transaction support.
            Connection = ConnectionType.Direct
        });
        Users = new LiteDbUserRepository(_database);
        Rooms = new LiteDbRoomRepository(_database, Users);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}
