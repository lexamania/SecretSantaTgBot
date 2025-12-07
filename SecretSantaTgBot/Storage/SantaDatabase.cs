using LiteDB;

using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage;

public class SantaDatabase: IDisposable
{
    private LiteDatabase _db;

    public ILiteCollection<Participant> Participants { get; private set; }
    public ILiteCollection<User> Users { get; private set; }
    public ILiteCollection<UserWishes> UserWishes { get; private set; }

    public SantaDatabase()
    {
        InitializeDB();
    }

    public void InitializeDB()
    {
        var dbPath = EnvVariables.DBPath;
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _db = new LiteDatabase(dbPath);
        Participants = _db.GetCollection<Participant>("participants");
        Users = _db.GetCollection<User>("users");
        UserWishes = _db.GetCollection<UserWishes>("user_wishes");
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
