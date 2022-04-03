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

    public async Task<IEnumerable<MessageDto>> GetTimelineByUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return new List<MessageDto>();
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

    public async Task<IEnumerable<MessageDto>> GetOwnTimelineById(long userId)
    {
        var user = await _userRepository.GetUserById(userId);
        if (userId > 0 && user != null)
        {
            return await GetTimelineByUsername(user.Username);
        }
        return await GetPublicTimeline();
    }
    
    public async Task<Status> PostNewMessageToTimeline(long userId, string text)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null && text != "")
        {
            return Status.NotFound;
        }
        await _context.Messages.AddAsync(new Message
        {
            Text = text,
            AuthorId = userId,
            Author = user,
            PubDate = DateTime.UtcNow.Ticks,
            Flagged = 0
        });
        await _context.SaveChangesAsync();
        return Status.Created;
    }
}