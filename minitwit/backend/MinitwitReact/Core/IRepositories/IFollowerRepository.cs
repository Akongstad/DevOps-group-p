namespace MinitwitReact.Core.IRepositories;

public interface IFollowerRepository
{
    Task<Status> FollowUser(long sessionId ,string username);
    Task<Status> UnfollowUser(long sessionId ,string username);
    Task<IEnumerable<UserDto>> GetFollowers(string username, int limit);
    Task<bool> IsFollowing(long sessionId, UserDto user);
}