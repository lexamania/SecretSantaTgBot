using LiteDB;

using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Storage.Repositories;

public class RoomRepository(LiteDatabase db) : DirectoryBase<PartyRoomEntity>(db, "rooms")
{
    public PartyRoomEntity? GetById(Guid id) => Collection.FindById(id);

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
