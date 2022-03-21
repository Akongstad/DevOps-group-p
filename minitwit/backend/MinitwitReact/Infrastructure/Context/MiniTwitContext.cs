namespace MinitwitReact.Entities
{
    public class MiniTwitContext : DbContext, IMiniTwitContext
    {
        public virtual DbSet<Follower> Followers => Set<Follower>();
        public virtual DbSet<Message> Messages => Set<Message>();
        public virtual DbSet<User> Users  => Set<User>();
        
        public MiniTwitContext(DbContextOptions<MiniTwitContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Follower>().HasKey(nameof(Follower.WhoId), nameof(Follower.WhomId));
            modelBuilder.Entity<User>().HasIndex(u => u.Id);

            modelBuilder.Entity<Message>().HasIndex(m => m.PubDate);
        }
    }
}
