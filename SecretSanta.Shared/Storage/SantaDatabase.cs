using LiteDB;

using SecretSanta.Shared.Extensions;
using SecretSanta.Shared.Storage.Repositories;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Utils;

namespace SecretSanta.Shared.Storage;

public class SantaDatabase: IDisposable
{
    private readonly LiteDatabase _database;

    public UserRepository Users { get; }
    public RoomRepository Rooms { get; }

    public SantaDatabase(string dbPath)
    {
        DirectoryUtils.CreateDirectoryRecursively(Path.GetDirectoryName(dbPath)!);

        _database = new LiteDatabase(dbPath);
        Users = new UserRepository(_database);
        Rooms = new RoomRepository(_database);
    }

    public PartyRoomEntity CreateRoom(
        UserEntity admin,
        string title,
        string description,
        string adminInRoomName)
    {
        var room = Rooms.Create(admin, title, description);
        AddUserToRoom(admin, room, adminInRoomName);
        return room;
    }

    public PartyRoomEntity DeleteRoom(UserEntity user, Guid roomId)
    {
        var room = user.AvailableRooms.First(x => roomId.Equals(x.Id));
        var userIds = room.Participants.Select(x => x.TelegramUserId).ToList();
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

    public void AddUserToRoom(
        UserEntity user,
        PartyRoomEntity room,
        string userInRoomName)
    {
        user.AvailableRooms.Add(room);
        room.Participants.Add(new()
        {
            TelegramUserId = user.TelegramUserId,
            Username = user.Username,
            CustomName = userInRoomName
        });

        Rooms.Update(room);
        Users.Update(user);
    }

    public void RemoveUserFromRoom(UserEntity user, PartyRoomEntity room)
    {
        var deletedRoom = user.AvailableRooms.RemoveFirst(x => x.Id == room.Id);
        var deletedParticipant = room.Participants.RemoveFirst(x => x.TelegramUserId == user.TelegramUserId);

        Users.ExitRoom(user);
        Rooms.Update(room);
    }

    public void Dispose()
    {
        _database.Dispose();
        GC.SuppressFinalize(this);
    }
}
