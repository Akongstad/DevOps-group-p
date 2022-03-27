namespace MinitwitReact;

public class Minitwit : IMinitwit, IDisposable
{
    private const int PageLimit = 100;

    private readonly IMinitwitContext _context;
    public Minitwit(IMinitwitContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<UserDto>> GetAllUsers() => await _context.Users.Select(u => new UserDto(u.UserId, u.Username)).ToListAsync();
    public async Task<UserDto?> GetUserById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid);
        if (user == null)
        {
            return null;
        }
        return new UserDto(user.UserId, user.Username);
    }

    public async Task<UserDetailsDto?> GetUserDetailsById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid);
        if (user == null)
        {
            return null;
        }

        return new UserDetailsDto(user.UserId, user.Username, user.Email, user.PwHash);
    }
    
    public async Task<long> GetUserId(string username)
    { 
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
       if (user == null)
       {
           return 0;
       }
       return user.UserId;
    }

    public async Task<IEnumerable<MessageDto>> PublicTimeline()
    {
        var messages = await _context.Messages
            .AsNoTracking()
            .Where(m => m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(PageLimit)
            .Select(m => new MessageDto(
                m.MessageId,
                m.Author!.Username,
                m.Text!,
                m.PubDate)).ToListAsync();
        return messages;
    }

    public async Task<bool> Follows(long sessionId, UserDto user)
    {
        var follows = await _context.Followers.Where(f => f.WhoId == sessionId && f.WhomId == user.UserId).ToListAsync();
        return follows.Count > 0;
    }
    public async Task<IEnumerable<MessageDto>> UserTimeline(long sessionId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return null!;
        }
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.AuthorId == user.UserId)
            .OrderByDescending(m => m.PubDate)
            .Take(PageLimit)
            .Select(m => new MessageDto(
                m.MessageId,
                m.Author!.Username,
                m.Text!,
                m.PubDate)).ToListAsync();
    }
    public async Task<IEnumerable<MessageDto>> OwnTimeline(long sessionId)
    {
        var user = await GetUserById(sessionId);
        if (sessionId > 0 && user != null)
        {
            return await UserTimeline(sessionId, user.Username);
        }
        return await PublicTimeline();
    }

    public async Task<IEnumerable<UserDto>> GetFollowers(string username, int limit)
    {
        var userId = await GetUserId(username);
        if (userId <= 0)
        {
            return new List<UserDto>();
        }
        return await (from u in _context.Users
            join f in _context.Followers on u.UserId equals f.WhomId
            where f.WhoId == userId
            select new UserDto(u.UserId, u.Username)).Take(limit).ToListAsync();
    }
    
    public async Task<Status> PostMessage(long userid, string text)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid);
        if (user == null)
        {
            return Status.NotFound;
        }
        await _context.Messages.AddAsync(new Message
        {
            Text = text,
            AuthorId = userid,
            Author = user,
            PubDate = DateTime.UtcNow.Ticks,
            Flagged = 0
        });
        await _context.SaveChangesAsync();
        return Status.Created;
    }
    public async Task<Status> FollowUser(long sessionId ,string username)
    {
        var ownUser = await GetUserDetailsById(sessionId);
        if (ownUser == null)
        {
            return Status.NotFound;
        }
        var whomId = await GetUserId(username);
        if (whomId == 0)
        {
            return Status.NotFound;
        }
        var whomUser = await GetUserDetailsById(whomId);
        if ( await Follows(sessionId, whomUser!) || whomId == sessionId)
        {
            return Status.Conflict;
        }

        await _context.Followers.AddAsync(new Follower {WhoId = sessionId, WhomId = whomId});
        await _context.SaveChangesAsync();
        return Status.Updated;
    }
    public async Task<Status> UnfollowUser(long sessionId ,string username)
    {
        var ownUser = await GetUserDetailsById(sessionId);
        if (ownUser is null)
        {
            return Status.NotFound;
        }
        var whomId = await GetUserId(username);
        if (whomId is 0)
        {
            return Status.NotFound;
        }
        var whomUser = await GetUserDetailsById(whomId);
        if ( !await Follows(sessionId, whomUser!) || whomId == sessionId)
        {
            return Status.Conflict;
        }
        var follower = await _context.Followers.FirstOrDefaultAsync(f => f.WhoId == sessionId && f.WhomId == whomId);
        _context.Followers.Remove(follower!);
        await _context.SaveChangesAsync();
        return Status.Updated;
        
    }

    public async Task<long> Login(string username, string pw)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
        {
            return 0;
        }
        if (!BCrypt.Net.BCrypt.Verify(pw, user.PwHash))
        {
            return -1;
        }
        return user.UserId;
    }

    public async Task<long> Register(string username, string email, string pw)
    {
        var conflict = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (conflict is not null)
        {
            return 0;
        }
        var user = new User {Username = username, Email = email, PwHash = BCrypt.Net.BCrypt.HashPassword(pw)};
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return savedUser!.UserId;
    }

    public async Task<UserDetailsDto?> UserByName(string name)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
        if (user is null)
        {
            return null;
        }
        return new UserDetailsDto(user.UserId, user.Username, user.Email, user.PwHash);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}