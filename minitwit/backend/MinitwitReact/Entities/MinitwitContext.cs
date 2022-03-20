namespace MinitwitReact.Entities
{
    public class MinitwitContext : DbContext, IMinitwitContext
    {
        public MinitwitContext(DbContextOptions<MinitwitContext> options) : base(options) { }

        public virtual DbSet<Follower> Followers => Set<Follower>();
        public virtual DbSet<Message> Messages => Set<Message>();
        public virtual DbSet<User> Users  => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Follower>().HasKey(nameof(Follower.WhoId), nameof(Follower.WhomId));
            modelBuilder.Entity<User>().HasIndex(u => u.UserId);

            modelBuilder.Entity<Message>().HasIndex(m => m.PubDate);
        }
    }
}
