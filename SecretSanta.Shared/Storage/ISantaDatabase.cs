using SecretSanta.Shared.Storage.Repositories;

namespace SecretSanta.Shared.Storage;

public interface ISantaDatabase : IDisposable
{
    IUserRepository Users { get; }
    IRoomRepository Rooms { get; }
}
