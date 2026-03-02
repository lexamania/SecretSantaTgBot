using LiteDB;

using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Storage.Directories;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage;

public class SantaDatabase: IDisposable
{
    private readonly LiteDatabase _database;

    public UserDirectory UserDirectory { get; }
    public RoomDirectory RoomDirectory { get; }

    public SantaDatabase()
    {
        var dbPath = EnvVariables.DBPath;
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _database = new LiteDatabase(dbPath);
        UserDirectory = new UserDirectory(_database);
        RoomDirectory = new RoomDirectory(_database);
    }

    public PartyRoomEntity DeleteRoom(UserEntity user, string roomId)
    {
        var room = user.AvailableRooms.First(x => roomId.Equals(x.Id.ToString()));
        var userIds = room.Users.Select(x => x.Id).ToList();
        var users = UserDirectory.GetByIds(userIds);

        foreach (var u in users)
        {
            var deletedRoom = u.AvailableRooms.First(r => r.Id == room.Id);
            u.AvailableRooms.Remove(deletedRoom);

            if (u.SelectedRoom?.Id == room.Id)
            {
                u.SelectedRoom = default;
                u.CurrentState = default;
            }
        }

        RoomDirectory.Delete(room.Id);
        UserDirectory.Update(users);

        return room;
    }

    public void LeaveRoom(UserEntity user, PartyRoomEntity room)
    {
        var deletedRoom = user.AvailableRooms.RemoveFirst(x => x.Id == room.Id);
        var deletedParticipant = room.Users.RemoveFirst(x => x.Id == user.Id);

        UserDirectory.UpdateWithClearRoom(user);
        RoomDirectory.Update(room);
    }

    public void JoinRoom(UserEntity user, PartyRoomEntity room)
    {
        user.AvailableRooms.Add(room);
        room.Users.Add(new()
        {
            Id = user.Id,
            Username = user.Username
        });

        RoomDirectory.Update(room);
        UserDirectory.Update(user);
    }

    public void Dispose()
    {
        _database.Dispose();
        GC.SuppressFinalize(this);
    }
}
