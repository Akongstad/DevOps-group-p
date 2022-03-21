namespace MinitwitReact.Core.IRepositories;

public interface IUserRepository
{
    Task<long> Login(string username, string pw);
    Task<long> Register(string username, string email, string pw);
    Task<IEnumerable<MessageDto>> GetPublicTimeline();
    Task<IEnumerable<MessageDto>> GetTimeline(long sessionId, string username);
    Task<IEnumerable<MessageDto>> GetOwnTimeline(long sessionId);
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<long> GetUserId(string username);
    Task<UserDetailsDto?> GetUserDetailsById(long userid);
    Task<UserDetailsDto?> GetUserByName(string name);
    Task<Status> PostMessage(long userid, string message);
}

