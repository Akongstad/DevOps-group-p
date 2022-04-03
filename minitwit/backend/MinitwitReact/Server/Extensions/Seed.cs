namespace MinitwitReact.Extensions;

public static class Seed
{
    public static async Task<IHost> SeedAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MiniTwitContext>();

            await SeedMinitwitAsync(context);
        }

        return host;
    }

    private static async Task SeedMinitwitAsync(MiniTwitContext context)
    {
        await context.Database.MigrateAsync();
    }
}