namespace MinitwitReact.Extensions;

public static class Seed
{
    public static async Task<IHost> SeedAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MinitwitContext>();

            await SeedMinitwitAsync(context);
        }

        return host;
    }

    public static async Task SeedMinitwitAsync(MinitwitContext context)
    {
        await context.Database.MigrateAsync();
        
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123"};
        var jeff = new User {Username = "Jeff Bezos", Email = "Amazon@gmail.com", PwHash = "321"};
        var bill = new User {Username = "Bill Gates", Email = "Microsoft@gmail.com", PwHash = "321123"};
        var bruce = new User{Username = "Bruce Wayne", Email = "Gotham@gmail.com", PwHash = "321"};
        
        var hello = new Message
        {
            Author = jeff, Flagged = 0, Text = "Elon bad",
            PubDate = DateTime.UtcNow.AddDays(3).Ticks
        };
        var bye = new Message
        {
            Author = elon, Flagged = 0, Text = "Tesla stonks",
            PubDate = DateTime.UtcNow.AddDays(1).Ticks
        };
        var batman = new Message
        {
            Author = bruce, Flagged = 0, Text = "I am Batman!",
            PubDate = DateTime.UtcNow.AddMinutes(1).Ticks
        };
        var chip = new Message
        {
            Author = bill, Flagged = 0, Text = "Get microsoft chip. Very good, very niice",
            PubDate = DateTime.UtcNow.AddDays(2).Ticks
        };
        context.Messages.AddRange(hello, bye, batman, chip);
        context.Followers.Add(new Follower {WhoId = 2, WhomId = 1});
        await context.SaveChangesAsync();
        
    }
}