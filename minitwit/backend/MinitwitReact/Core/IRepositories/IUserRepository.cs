namespace MinitwitReact.Core.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<UserDetailsDto?> GetUserDetailsById(long userId);
    Task<long> GetUserIdFromUsername(string userName);
    Task<UserDetailsDto?> GetUserDetailsByName(string userName);
    Task<AuthStatus> Login(UserLoginDto userLoginDto);
    Task<AuthStatus> Register(UserCreateDto userCreateDto);
}