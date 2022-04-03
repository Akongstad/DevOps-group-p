namespace MinitwitReact.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMiniTwitContext _context;

    public UserRepository(IMiniTwitContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserDto>> GetAllUsers() => 
        await _context.Users.Select(u => new UserDto(u.Id, u.Username)).ToListAsync();
    
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