namespace MinitwitReact.Core.IRepositories;

public interface IFollowerRepository
{
    Task<Status> FollowUser(FollowerDto followRelation);
    Task<Status> UnfollowUser(FollowerDto followRelation);
    Task<IEnumerable<UserDto>> GetFollowersByUsernameWithLimit(string username, int limit);
    Task<bool> IsFollowing(long newFollowerId, UserDto targetUser);
}