using LiteDB;

using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Storage.Directories;

public class RoomDirectory(LiteDatabase db) : DirectoryBase<PartyRoomEntity>(db, "rooms")
{
    public PartyRoomEntity? GetById(Guid id) => Collection.FindById(id);

    public PartyRoomEntity Create(UserEntity user, string title, string description)
    {
        var participant = new ParticipantEntity()
        {
            Id = user.Id,
            Username = user.Username,
        };

        var room = new PartyRoomEntity
        {
            Title = title,
            PartyDescription = description,
            Admin = user,
            Users = [participant],
        };

        Collection.Insert(room);
        return room;
    }

    public bool Update(PartyRoomEntity room) => Collection.Update(room);
    public bool Delete(Guid id) => Collection.Delete(id);
}
