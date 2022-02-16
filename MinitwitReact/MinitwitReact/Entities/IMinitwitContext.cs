using Microsoft.EntityFrameworkCore;

namespace MinitwitReact.Entities;

public interface IMinitwitContext : IDisposable
{
    DbSet<Follower> Followers { get; }
    DbSet<Message> Messages { get; }
    DbSet<User> Users { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}