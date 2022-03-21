namespace MinitwitReact.Infrastructure;

public class Authenticator : IAuthenticator
{
    private readonly IMiniTwitContext _context;

    public Authenticator(IMiniTwitContext context)
    {
        _context = context;
    }
    public async Task<long> Login(string username, string pw)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
        {
            return 0;
        }
        if (!BCrypt.Net.BCrypt.Verify(pw, user.PwHash))
        {
            return -1;
        }
        return user.Id;
    }

    public async Task<long> Register(string username, string email, string pw)
    {
        var conflict = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (conflict is not null)
        {
            return 0;
        }
        var user = new User {Username = username, Email = email, PwHash = BCrypt.Net.BCrypt.HashPassword(pw)};
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return savedUser!.Id;
    }
}