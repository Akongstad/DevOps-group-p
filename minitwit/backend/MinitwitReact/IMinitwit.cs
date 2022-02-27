namespace MinitwitReact;

public interface IMinitwit
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(long userId);
    Task<long> GetUserId(string username);
    Task<UserDetailsDto?> GetUserDetialsById(long userid);
    Task<DateTime> FormatDatetime(string timestamp); //Can probably be done using datetime
    Uri gravatar_url(string email, int size = 80);
    Task<Status> PostMessage(long userid, string message);
    Task<Status> FollowUser(long sessionId ,string username);
    Task<Status> UnfollowUser(long sessionId ,string username);
    Task<long> Login(string username, string pw);
    Task<long> Register(string username, string email, string pw);
    Task<IEnumerable<Tuple<MessageDto, UserDto>>> PublicTimeline();
    Task<IEnumerable<Tuple<MessageDto, UserDto>>> UserTimeline(long sessionId, string username);
    Task<bool> Follows(long sessionId, UserDto user);
    Task<IEnumerable<Tuple<MessageDto, UserDto>>> OwnTimeline(long sessionId);

    Task<IEnumerable<UserDto>> GetFollowers(string username, int limit);
}