namespace MinitwitReact.Entities;

public interface IMiniTwitContext : IDisposable
{
    DbSet<Follower> Followers { get; }
    DbSet<Message> Messages { get; }
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}