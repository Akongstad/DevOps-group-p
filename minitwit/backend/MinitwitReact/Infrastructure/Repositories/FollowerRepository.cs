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
    
    public async Task<Status> FollowUser(long sessionId ,string username)
    {
        var ownUser = await _userRepository.GetUserDetailsById(sessionId);
        if (ownUser == null)
        {
            return Status.NotFound;
        }
        var whomId = await _userRepository.GetUserIdFromUsername(username);
        if (whomId == 0)
        {
            return Status.NotFound;
        }
        var whomUser = await _userRepository.GetUserDetailsById(whomId);
        if ( await IsFollowing(sessionId, whomUser!) || whomId == sessionId)
        {
            return Status.Conflict;
        }

        await _context.Followers.AddAsync(new Follower {WhoId = sessionId, WhomId = whomId});
        await _context.SaveChangesAsync();
        return Status.Updated;
    }
    
    public async Task<Status> UnfollowUser(long sessionId ,string username)
    {
        var ownUser = await _userRepository.GetUserDetailsById(sessionId);
        if (ownUser is null)
        {
            return Status.NotFound;
        }
        var whomId = await _userRepository.GetUserIdFromUsername(username);
        if (whomId is 0)
        {
            return Status.NotFound;
        }
        var whomUser = await _userRepository.GetUserDetailsById(whomId);
        if ( !await IsFollowing(sessionId, whomUser!) || whomId == sessionId)
        {
            return Status.Conflict;
        }
        var follower = await _context.Followers.FirstOrDefaultAsync(f => f.WhoId == sessionId && f.WhomId == whomId);
        _context.Followers.Remove(follower!);
        await _context.SaveChangesAsync();
        return Status.Updated;
        
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowers(string username, int limit)
    {
        var userId = await _userRepository.GetUserIdFromUsername(username);
        if (userId <= 0)
        {
            return new List<UserDto>();
        }
        return await (from u in _context.Users
            join f in _context.Followers on u.Id equals f.WhomId
            where f.WhoId == userId
            select new UserDto(u.Id, u.Username)).Take(limit).ToListAsync();
    }
    
    public async Task<bool> IsFollowing(long sessionId, UserDto user)
    {
        var follows = await _context.Followers.Where(f => f.WhoId == sessionId && f.WhomId == user.UserId).ToListAsync();
        return follows.Count > 0;
    }
}