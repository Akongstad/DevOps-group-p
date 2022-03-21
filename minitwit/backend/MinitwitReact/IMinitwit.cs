namespace MinitwitReact;

public interface IMinitwit
{
    // User
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<long> GetUserId(string username);
    Task<UserDetailsDto?> GetUserDetailsById(long userid);
    Task<UserDetailsDto?> UserByName(string name);
    Task<Status> PostMessage(long userid, string message);
    
    // Follower
    Task<Status> FollowUser(long sessionId ,string username);
    Task<Status> UnfollowUser(long sessionId ,string username);
    Task<IEnumerable<UserDto>> GetFollowers(string username, int limit);
    Task<bool> Follows(long sessionId, UserDto user);


    
    
    Task<long> Login(string username, string pw);
    Task<long> Register(string username, string email, string pw);
    Task<IEnumerable<MessageDto>> PublicTimeline();
    Task<IEnumerable<MessageDto>> UserTimeline(long sessionId, string username);
    Task<IEnumerable<MessageDto>> OwnTimeline(long sessionId);
}