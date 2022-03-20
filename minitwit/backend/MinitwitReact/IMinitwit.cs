namespace MinitwitReact;

public interface IMinitwit
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<long> GetUserId(string username);
    Task<UserDetailsDto?> GetUserDetailsById(long userid);
    Task<Status> PostMessage(long userid, string message);
    Task<Status> FollowUser(long sessionId ,string username);
    Task<Status> UnfollowUser(long sessionId ,string username);
    Task<long> Login(string username, string pw);
    Task<long> Register(string username, string email, string pw);
    Task<IEnumerable<MessageDto>> PublicTimeline();
    Task<IEnumerable<MessageDto>> UserTimeline(long sessionId, string username);
    Task<bool> Follows(long sessionId, UserDto user);
    Task<IEnumerable<MessageDto>> OwnTimeline(long sessionId);
    Task<UserDetailsDto?> UserByName(string name);

    Task<IEnumerable<UserDto>> GetFollowers(string username, int limit);
}