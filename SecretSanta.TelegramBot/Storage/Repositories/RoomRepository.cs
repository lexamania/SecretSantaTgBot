using LiteDB;

using SecretSanta.TelegramBot.Storage.Entities;

namespace SecretSanta.TelegramBot.Storage.Repositories;

public class RoomRepository(LiteDatabase db) : DirectoryBase<PartyRoomEntity>(db, "rooms")
{
    public PartyRoomEntity? GetById(string id) => Guid.TryParse(id, out var roomId)
        ? Collection.Include(x => x.Admin).FindById(roomId)
        : default;

    public PartyRoomEntity Create(
        UserEntity admin,
        string title,
        string description)
    {
        var room = new PartyRoomEntity
        {
            Title = title,
            Description = description,
            Admin = admin,
        };

        Collection.Insert(room);
        return room;
    }

    public bool Update(PartyRoomEntity room) => Collection.Update(room);
    public bool Delete(Guid id) => Collection.Delete(id);
}
