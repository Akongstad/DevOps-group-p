namespace Minitwit.Tests.InfrastructureTests;

public class RepositorySetup
{
    private MiniTwitContext _context;
    public RepositorySetup()
    {
        //Setup for EF Core
        var conn = new SqliteConnection("Filename=:memory:");
        conn.Open();
        var builder = new DbContextOptionsBuilder<MiniTwitContext>();
        builder.UseSqlite(conn);
        _context = new MiniTwitContext(builder.Options);
        _context.Database.EnsureCreated();

        //Seed some stuff
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var jeff = new User {Username = "Jeff Bezos", Email = "Amazon@gmail.com", PwHash = "321", UserId = 2};
        var bill = new User {Username = "Bill Gates", Email = "Microsoft@gmail.com", PwHash = "321123", UserId = 3};
        var bruce = new User{Username = "Bruce Wayne", Email = "Gotham@gmail.com", PwHash = "321", UserId = 4};
        var hashman = new User
            {Username = "Hash Tester", Email = "Hash@live.com", PwHash = BCrypt.Net.BCrypt.HashPassword("hashed"), UserId = 5};
        
        var hello = new Message
        {
            Author = jeff, AuthorId = 2, Flagged = 0, Text = "Elon bad",
            PubDate = DateTime.UtcNow.AddDays(3).Ticks
        };
        var bye = new Message
        {
            Author = elon, AuthorId = 1, Flagged = 0, Text = "Tesla stonks",
            PubDate = DateTime.UtcNow.AddDays(1).Ticks
        };
        var batman = new Message
        {
            Author = bruce, AuthorId = 4, Flagged = 0, Text = "I am Batman!",
            PubDate = DateTime.UtcNow.AddMinutes(1).Ticks
        };
        var chip = new Message
        {
            Author = bill, AuthorId = 3, Flagged = 0, Text = "Get microsoft chip. Very good, very niice",
            PubDate = DateTime.UtcNow.AddDays(2).Ticks
        };
        var strong = new Message
        {
            Author = hashman, Flagged = 0, Text = "I have strong password", PubDate = DateTime.UtcNow.AddHours(1).Ticks
        };
        _context.Messages.AddRange(hello, bye, batman, chip, strong);
        _context.Followers.Add(new Follower {WhoId = 2, WhomId = 1});

        _context.SaveChanges();
    }

    public IMiniTwitContext GetTwitContext()
    {
        return _context;
    }
}