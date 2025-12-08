using LiteDB;

using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage;

public class SantaDatabase: IDisposable
{
    private LiteDatabase _db;

    public ILiteCollection<UserTg> Users { get; private set; }
    public ILiteCollection<PartyRoom> Rooms { get; private set; }

    public SantaDatabase()
    {
        InitializeDB();
    }

    public void InitializeDB()
    {
        var dbPath = EnvVariables.DBPath;
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _db = new LiteDatabase(dbPath);
        Users = _db.GetCollection<UserTg>("users");
        Rooms = _db.GetCollection<PartyRoom>("rooms");
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
