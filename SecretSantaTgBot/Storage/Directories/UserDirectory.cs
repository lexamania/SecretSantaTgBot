using LiteDB;

using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Storage.Directories;

public class UserDirectory(LiteDatabase db) : DirectoryBase<UserEntity>(db, "users")
{
    private ILiteCollection<UserEntity> FullUsers => Collection
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom);

    public UserEntity GetById(long id)
        => FullUsers.FindById(id);

    public List<UserEntity> GetByIds(IEnumerable<long> ids)
        => [.. FullUsers.Find(x => ids.Contains(x.Id))];

    public UserEntity GetOrCreate(Chat chat)
    {
        var user = GetById(chat.Id);
        if (user is null)
            return Create(chat.Id, chat.Username!);

        if (user.Username != chat.Username)
        {
            user.Username = chat.Username!;
            Update(user);
        }

        return user;
    }

    public UserEntity Create(long id, string username)
    {
        var user = new UserEntity()
        {
            Id = id,
            Username = username,
            AvailableRooms = []
        };

        Collection.Insert(user);
        return user;
    }

    public bool Update(UserEntity user) => Collection.Update(user);
    public void Update(IEnumerable<UserEntity> users) => Collection.Update(users);

    public bool UpdateWithClearState(UserEntity user) => UpdateWithState(user, default);
    public bool UpdateWithState(UserEntity user, string? state)
    {
        user.CurrentState = state;
        return Update(user);
    }

    public bool UpdateWithClearRoom(UserEntity user)
    {
        user.SelectedRoom = default;
        user.CurrentState = default;
        return Update(user);
    }
}
