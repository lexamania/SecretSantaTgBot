using LiteDB;

using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

namespace SecretSantaTgBot.Storage.Repositories;

public class UserRepository(LiteDatabase db) : DirectoryBase<UserEntity>(db, "users")
{
    private ILiteCollection<UserEntity> FullUsers => Collection
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom);

    public List<UserEntity> GetByIds(IEnumerable<long> ids)
        => [.. FullUsers.Find(x => ids.Contains(x.TelegramUserId))];

    public UserEntity GetOrCreate(long id, string? username, string? language = null)
    {
        var user = FullUsers.FindById(id);

        if (user is null)
        {
            user = new UserEntity()
            {
                TelegramUserId = id,
                Username = username,
                PrefferedLanguage = LanguageUtil.ConvertLanguageToSystem(language),
                AvailableRooms = []
            };

            Collection.Insert(user);
        }

        if (username is not null && user.Username?.Equals(username) != true)
        {
            user.Username = username;
            Update(user);
        }

        if (user.PrefferedLanguage is null && language is not null)
        {
            user.PrefferedLanguage = language;
            Update(user);
        }

        return user;
    }

    public bool Update(UserEntity user) => Collection.Update(user);
    public void Update(IEnumerable<UserEntity> users) => Collection.Update(users);

    public bool SetState(UserEntity user, string state)
    {
        user.CurrentState = state;
        return Update(user);
    }

    public bool ClearState(UserEntity user)
    {
        user.CurrentState = default;
        return Update(user);
    }

    public bool EnterRoom(UserEntity user, PartyRoomEntity room)
    {
        user.SelectedRoom = room;
        user.CurrentState = default;
        return Update(user);
    }

    public bool ExitRoom(UserEntity user)
    {
        user.SelectedRoom = default;
        user.CurrentState = default;
        return Update(user);
    }
}
