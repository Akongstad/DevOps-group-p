namespace MinitwitReact.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMiniTwitContext _context;
    private readonly IUserRepository _userRepository;
    private const int PageLimit = 100;

    public MessageRepository(IMiniTwitContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
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

    public async Task<IEnumerable<MessageDto>> GetTimelineByUsernameAndSessionId(long sessionId, string username)
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

    public async Task<IEnumerable<MessageDto>> GetOwnTimelineBySessionId(long sessionId)
    {
        var user = await _userRepository.GetUserById(sessionId);
        if (sessionId > 0 && user != null)
        {
            return await GetTimelineByUsernameAndSessionId(sessionId, user.Username);
        }
        return await GetPublicTimeline();
    }
    
    public async Task<Status> PostMessageToTimeline(long userid, string text)
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