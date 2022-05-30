namespace MinitwitReact.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMiniTwitContext _context;

    public UserRepository(IMiniTwitContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserDto>> GetAllUsers() => 
        await _context.Users.Select(u => new UserDto(u.UserId, u.Username)).ToListAsync();
    
    public async Task<UserDto?> GetUserById(long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        return user == null ? null : new UserDto(user.UserId, user.Username);
    }
        
    public async Task<UserDetailsDto?> GetUserDetailsById(long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        return user == null ? null : new UserDetailsDto(user.UserId, user.Username, user.Email, user.PwHash);
    }
    
    public async Task<long> GetUserIdFromUsername(string userName)
    { 
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        return user?.UserId ?? 0;
    }
    
    public async Task<UserDetailsDto?> GetUserDetailsByName(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        return user is null ? null : new UserDetailsDto(user.UserId, user.Username, user.Email, user.PwHash);
    }
    
    public async Task<AuthStatus> Login(UserLoginDto userLoginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username);
        if (user is null)
        {
            return AuthStatus.WrongUsername;
        }
        return !BCrypt.Net.BCrypt.Verify(userLoginDto.PwHash, user.PwHash) ? AuthStatus.WrongPassword : AuthStatus.Authorized;
    }

    public async Task<AuthStatus> Register(UserCreateDto userCreateDto)
    {
        var candidateUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == userCreateDto.Username);
        if (candidateUsername is not null)
        {
            return AuthStatus.UsernameInUse;
        }
        var user = new User {Username = userCreateDto.Username, Email = userCreateDto.Email, PwHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.PwHash)};
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return AuthStatus.Authorized;
    }
}