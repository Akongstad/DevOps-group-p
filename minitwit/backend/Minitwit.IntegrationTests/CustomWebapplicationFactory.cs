namespace Minitwit.IntegrationTests;

public class CustomWebApplicationFactory :  WebApplicationFactory<Program>
{
    public string DefaultUserId { get; set; } = "1";
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Setup Test Auth handler
            services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
            
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MinitwitContext>));

            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            var connection = new SqliteConnection("Filename=:memory:");

            services.AddDbContext<MinitwitContext>(options =>
            {
                options.UseSqlite(connection);
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<MinitwitContext>();
            appContext.Database.OpenConnection();
            appContext.Database.EnsureCreated();

            SeedProjects(appContext);
        });

        builder.UseEnvironment("Integration");

        return base.CreateHost(builder);
    }

    private static void SeedProjects(MinitwitContext context)
    {
        //Seed some stuff
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var jeff = new User {Username = "Jeff Bezos", Email = "Amazon@gmail.com", PwHash = "321", UserId = 2};
        var bill = new User {Username = "Bill Gates", Email = "Microsoft@gmail.com", PwHash = "321123", UserId = 3};
        var bruce = new User{Username = "Bruce Wayne", Email = "Gotham@gmail.com", PwHash = "321", UserId = 4};
        
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
        context.Messages.AddRange(hello, bye, batman, chip);
        context.Followers.Add(new Follower {WhoId = 2, WhomId = 1});

        context.SaveChanges();

    }
}