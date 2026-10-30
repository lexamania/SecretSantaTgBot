using LiteDB;

using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Storage.Repositories;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage;

public class SantaDatabase: IDisposable
{
    private readonly LiteDatabase _database;

    public UserRepository Users { get; }
    public RoomRepository Rooms { get; }

    public SantaDatabase(BotConfiguration config)
    {
        var dbPath = config.DBPath;
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

        room.Participants.Add(new()
        {
            TargetUserId = admin.TelegramUserId,
            Username = admin.Username,
            InRoomName = adminInRoomName
        });
        admin.AvailableRooms.Add(room);

        Users.Update(admin);
        Rooms.Update(room);

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
            InRoomName = userInRoomName
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
