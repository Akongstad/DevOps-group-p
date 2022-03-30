namespace MinitwitReact.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMiniTwitContext _context;

    public UserRepository(IMiniTwitContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserDto>> GetAllUsers() => await _context.Users.Select(u => new UserDto(u.Id, u.Username)).ToListAsync();
    
    public async Task<UserDto?> GetUserById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
        return user == null ? null : new UserDto(user.Id, user.Username);
    }
        
    public async Task<UserDetailsDto?> GetUserDetailsById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
        return user == null ? null : new UserDetailsDto(user.Id, user.Username, user.Email, user.PwHash);
    }
    
    public async Task<long> GetUserIdFromUsername(string username)
    { 
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user?.Id ?? 0;
    }
    
    public async Task<UserDetailsDto?> GetUserDetailsByName(string name)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
        return user is null ? null : new UserDetailsDto(user.Id, user.Username, user.Email, user.PwHash);
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