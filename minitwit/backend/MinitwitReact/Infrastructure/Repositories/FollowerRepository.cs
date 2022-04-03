namespace MinitwitReact.Infrastructure.Repositories;

public class FollowerRepository : IFollowerRepository
{
    private readonly IMiniTwitContext _context;
    private readonly IUserRepository _userRepository;

    public FollowerRepository(IMiniTwitContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }
    
    public async Task<Status> FollowUser(FollowerDto followRelation)
    {
        var newFollowerId = await _userRepository.GetUserDetailsById(followRelation.FollowerId);
        var targetId = await _userRepository.GetUserIdFromUsername(followRelation.FollowedUsername);
        
        // Checks that both users are valid
        if (targetId == 0 || newFollowerId is null)
        {
            return Status.NotFound;
        }
        
        // Checks that targetId is not the same as newFollowerId and that newFollowerId is not already following
        if ( await IsFollowing(followRelation.FollowerId, (await _userRepository.GetUserDetailsById(targetId))!) || targetId == followRelation.FollowerId)
        {
            return Status.Conflict;
        }

        await _context.Followers.AddAsync(new Follower {WhoId = followRelation.FollowerId, WhomId = targetId});
        await _context.SaveChangesAsync();
        return Status.Updated;
    }
    
    public async Task<Status> UnfollowUser(FollowerDto followRelation)
    {
        var newFollowerId = await _userRepository.GetUserDetailsById(followRelation.FollowerId);
        var targetId = await _userRepository.GetUserIdFromUsername(followRelation.FollowedUsername);
        
        // Checks that both users are valid
        if (newFollowerId is null || targetId is 0)
        {
            return Status.NotFound;
        }
        
        // Checks that targetId is not the same as newFollowerId and that newFollowerId is following
        if ( !await IsFollowing(followRelation.FollowerId, (await _userRepository.GetUserDetailsById(targetId))!) || targetId == followRelation.FollowerId)
        {
            return Status.Conflict;
        }
        
        var follower = await _context.Followers
            .FirstOrDefaultAsync(f => f.WhoId == followRelation.FollowerId && f.WhomId == targetId);
        _context.Followers.Remove(follower!);
        
        await _context.SaveChangesAsync();
        return Status.Updated;
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowersByUsernameWithLimit(string username, int limit)
    {
        var userId = await _userRepository.GetUserIdFromUsername(username);
        if (userId <= 0)
        {
            return new List<UserDto>();
        }
        return await (from u in _context.Users
            join f in _context.Followers on u.UserId equals f.WhomId
            where f.WhoId == userId
            select new UserDto(u.UserId, u.Username)).Take(limit).ToListAsync();
    }
    
    public async Task<bool> IsFollowing(long newFollowerId, UserDto targetUser)
    {
        var follows = 
        await _context.Followers.FirstOrDefaultAsync(f => f.WhoId == newFollowerId && f.WhomId == targetUser.UserId);
        return follows != null;
    }
}