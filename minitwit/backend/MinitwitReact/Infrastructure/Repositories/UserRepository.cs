namespace MinitwitReact.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private const int PageLimit = 100;
    private readonly IMiniTwitContext _context;
    public UserRepository(IMiniTwitContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserDto>> GetAllUsers() => await _context.Users.Select(u => new UserDto(u.Id, u.Username)).ToListAsync();
    
    public async Task<UserDto?> GetUserById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
        if (user == null)
        {
            return null;
        }
        return new UserDto(user.Id, user.Username);
    }
        
    public async Task<UserDetailsDto?> GetUserDetailsById(long userid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
        if (user == null)
        {
            return null;
        }

        return new UserDetailsDto(user.Id, user.Username, user.Email, user.PwHash);
    }
    
    public async Task<long> GetUserIdFromName(string username)
    { 
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return 0;
        }
        return user.Id;
    }
    
    public async Task<UserDetailsDto?> GetUserByName(string name)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
        return user is null ? null : new UserDetailsDto(user.Id, user.Username, user.Email, user.PwHash);
    }


    public async Task<IEnumerable<MessageDto>> GetPublicTimeline()
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
    
    public async Task<IEnumerable<MessageDto>> GetTimeline(long sessionId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return null!;
        }
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.AuthorId == user.Id)
            .OrderByDescending(m => m.PubDate)
            .Take(PageLimit)
            .Select(m => new MessageDto(
                m.MessageId,
                m.Author!.Username,
                m.Text!,
                m.PubDate)).ToListAsync();
    }
    
    public async Task<IEnumerable<MessageDto>> GetOwnTimeline(long sessionId)
    {
        var user = await GetUserById(sessionId);
        if (sessionId > 0 && user != null)
        {
            return await GetTimeline(sessionId, user.Username);
        }
        return await GetPublicTimeline();
    }
    
    public async Task<Status> PostMessage(long userid, string text)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
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
}