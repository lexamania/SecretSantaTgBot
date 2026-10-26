using LiteDB;

using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Storage.Directories;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage;

public class SantaDatabase: IDisposable
{
    private readonly LiteDatabase _database;

    public UserDirectory Users { get; }
    public RoomDirectory Rooms { get; }

    public SantaDatabase(BotConfiguration config)
    {
        var dbPath = config.DBPath;
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _database = new LiteDatabase(dbPath);
        Users = new UserDirectory(_database);
        Rooms = new RoomDirectory(_database);
    }

    public PartyRoomEntity DeleteRoom(UserEntity user, string roomId)
    {
        var room = user.AvailableRooms.First(x => roomId.Equals(x.Id.ToString()));
        var userIds = room.Participants.Select(x => x.Id).ToList();
        var users = Users.GetByIds(userIds);

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

        Rooms.Delete(room.Id);
        Users.Update(users);

        return room;
    }

    public void RemoveUserFromRoom(UserEntity user, PartyRoomEntity room)
    {
        var deletedRoom = user.AvailableRooms.RemoveFirst(x => x.Id == room.Id);
        var deletedParticipant = room.Participants.RemoveFirst(x => x.Id == user.Id);

        Users.UpdateWithRoomClearing(user);
        Rooms.Update(room);
    }

    public void AddUserToRoom(UserEntity user, PartyRoomEntity room)
    {
        user.AvailableRooms.Add(room);
        room.Participants.Add(new()
        {
            Id = user.Id,
            Username = user.Username
        });

        Rooms.Update(room);
        Users.Update(user);
    }

    public void Dispose()
    {
        _database.Dispose();
        GC.SuppressFinalize(this);
    }
}
