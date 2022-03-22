namespace MinitwitReact.Core.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<UserDetailsDto?> GetUserDetailsById(long userid);

    Task<long> GetUserIdFromUsername(string username);
    Task<UserDetailsDto?> GetUserDetailsByName(string name);
}

