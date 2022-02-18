using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using static Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace MinitwitReact;

using Microsoft.Data.Sqlite;

public class Minitwit : IMinitwit, IDisposable
{
// configuration
    const string DATABASE = "Data Source=./../../minitwit.db";
    // docker
    //const string DATABASE = "Data Source=./../../minitwit.db";

    const int PER_PAGE = 30;
    const bool DEBUG = true;
    const string SECRET_KEY = "development key";
    
    private readonly IMinitwitContext _context;
    public Minitwit(IMinitwitContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<UserDto>> GetAllUsers() => await _context.Users.Select(u => new UserDto(u.UserId, u.Username)).ToListAsync();
    public async Task<UserDto?> GetUserById(long userid)
    {
        var users = from u in _context.Users
                    where u.UserId == userid
                    select new UserDto(u.UserId, u.Username);

        return await users.FirstOrDefaultAsync<UserDto>();
    }

    public async Task<UserDetailsDto?> GetUserDetialsById(long userid)
    {
        var users = from u in _context.Users
            where u.UserId == userid
            select new UserDetailsDto(u.UserId, u.Username, u.Email, u.PwHash);

        return await users.FirstOrDefaultAsync<UserDetailsDto>();
        
    }
    
    public async Task<long> GetUserIdEf(string username)
    {
       var users = from u in _context.Users
                   where u.Username == username
                   select new UserDto(u.UserId, u.Username);
        
       var user = await users.FirstOrDefaultAsync<UserDto>();
       if (user == null)
       {
           return 0;
       }
       return user.UserId;
    }
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> PublicTimeline()
    {
        var timeline = from m in _context.Messages
            join u in _context.Users on m.AuthorId equals u.UserId
            where m.Flagged == 0
            orderby m.PubDate descending

            select new {m, u}; // had errors when changing this line, changing it to DTO's below seemed to compile
        var reformat = timeline.Select(i => new Tuple<MessageDto, UserDto>(new MessageDto(i.m.MessageId, i.m.Author.Username, i.m.Text, i.m.PubDate), 
                                                                                                                new UserDto(i.u.UserId, i.u.Username)));
        var ordered = await reformat.ToListAsync();
        return ordered.OrderBy(m => m.Item1.PubDate);
    }
    
    public async Task<bool> Follows(long sessionId, UserDto user)
    {
        var follows = await _context.Followers.Where(f => f.WhoId == sessionId && f.WhomId == user.UserId).ToListAsync();
        return follows.Count > 0;
    }
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> UserTimeline(long sessionId, string username)
    {
        var users = from u in _context.Users
                   where u.Username == username
                   select new UserDto(u.UserId, u.Username);
        
        var user = await users.FirstOrDefaultAsync<UserDto>();
        if (user == null)
        {
            return null!;
        }
        var follows = await Follows(sessionId, user);
        /*if (!follows)
        {
            return await PublicTimelineEf();
        }*/
        var timeline = from m in _context.Messages
            join u in _context.Users on m.AuthorId equals u.UserId
            where u.UserId == m.AuthorId
            where u.UserId == user.UserId
            select new {m, u};

        var reformat = timeline.Select(i => new Tuple<MessageDto, UserDto>(new MessageDto(i.m.MessageId, i.m.Author.Username, i.m.Text, i.m.PubDate), 
                                                                                new UserDto(i.u.UserId, i.u.Username)));
        var ordered = await reformat.ToListAsync();
        return ordered.OrderBy(m => m.Item1.PubDate);
    }
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> OwnTimeline(long sessionId)
    {
        var user = await GetUserById(sessionId);
        if (sessionId > 0 && user != null)
        {
            return await UserTimeline(sessionId, user.Username);
        }
        return await PublicTimeline();
    }
    public async Task<DateTime> FormatDatetime(string timestamp)
    {
        return DateTime.Parse(timestamp);
        //.utcfromtimestamp(timestamp).strftime('%Y-%m-%d @ %H:%M')
    }
    // TODO: Make a Datetime convert from Datetime.Now to string

    // Return the gravatar image for the given email address.
    public Uri gravatar_url(string email, int size = 80)
    {
        var emailTrim = email.ToLower().Trim();
        return new Uri($"http://www.gravatar.com/avatar/{emailTrim}?d=identicon&s={size}");
    }
    public async Task<Status> PostMessageEf(long userid, string text)
    {
        var user = await GetUserDetialsById(userid);
        if (user == null)
        {
            return Status.NotFound;
        }
        await _context.Messages.AddAsync(new Message
        {
            Text = text,
            AuthorId = userid,
            Author = await _context.Users.FindAsync(userid),
            PubDate = DateTime.UtcNow.Ticks,
            Flagged = 0
        });
        await _context.SaveChangesAsync();
        return Status.Created;
    }
    public async Task<Status> FollowUserEf(long sessionId ,string username)
    {
        var ownUser = GetUserDetialsById(sessionId);
        if (ownUser == null)
        {
            return Status.NotFound;
        }
        var whomId = await GetUserIdEf(username);
        if (whomId == 0)
        {
            return Status.NotFound;
        }
        var whomUser = await GetUserDetialsById(whomId);
        if ( await Follows(sessionId, whomUser))
        {
            return Status.Conflict;
        }

        await _context.Followers.AddAsync(new Follower {WhoId = sessionId, WhomId = whomId});
        await _context.SaveChangesAsync();
        return Status.Updated;
    }
    public async Task<Status> UnfollowUserEf(long sessionId ,string username)
    {
        var ownUser = GetUserDetialsById(sessionId);
        if (ownUser == null)
        {
            return Status.NotFound;
        }
        var whomId = await GetUserIdEf(username);
        if (whomId == 0)
        {
            return Status.NotFound;
        }
        var whomUser = await GetUserDetialsById(whomId);
        if ( !await Follows(sessionId, whomUser))
        {
            return Status.Conflict;
        }
        var follower = await _context.Followers.FirstOrDefaultAsync(f => f.WhoId == sessionId && f.WhomId == whomId);
        _context.Followers.Remove(follower!);
        await _context.SaveChangesAsync();
        return Status.Updated;
        
    }

    public async Task<long> LoginEf(string username, string pw)
    {
        var userid = await GetUserIdEf(username);
        if (userid <= 0)
        {
            return 0;
        }
        var user = await GetUserDetialsById(userid);
        if (pw != user.PwHash)
        {
            return -1;
        }
        return userid;
    }

    public async Task<long> RegisterEf(string username, string email, string pw)
    {
        var conflict = await GetUserIdEf(username);
        if (conflict > 0)
        {
            return 0;
        }
        var user = new User {Username = username, Email = email, PwHash = pw};
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return savedUser.UserId;
    } 
    public void Dispose()
    {
        _context.Dispose();
    }
}