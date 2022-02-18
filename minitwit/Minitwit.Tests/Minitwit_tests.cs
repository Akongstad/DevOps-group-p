using MinitwitReact.Core;

namespace Minitwit.Tests;

public class MinitwitTests : IDisposable
{
    private readonly IMinitwit _minitwit;
    private readonly IMinitwitContext _context;
    public MinitwitTests()
    {
        //Setup for EF Core
        var conn = new SqliteConnection("Filename=:memory:");
        conn.Open();
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseSqlite(conn);
        var context = new MinitwitContext(builder.Options);
        context.Database.EnsureCreated();

        //Seed some stuff
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var jeff = new User {Username = "Jeff Bezos", Email = "Amazon@gmail.com", PwHash = "321", UserId = 2};
        var bill = new User {Username = "Bill Gates", Email = "Microsoft@gmail.com", PwHash = "321123", UserId = 3};
        var bruce = new User{Username = "Bruce Wayne", Email = "Gotham@gmail.com", PwHash = "321", UserId = 4};
        
        var hello = new Message
        {
            Author = jeff, AuthorId = 2, Flagged = 0, Text = "Elon bad",
            PubDate = DateTime.UtcNow.AddDays(3).Ticks
        };
        var bye = new Message
        {
            Author = elon, AuthorId = 1, Flagged = 0, Text = "Tesla stonks",
            PubDate = DateTime.UtcNow.AddDays(1).Ticks
        };
        var batman = new Message
        {
            Author = bruce, AuthorId = 4, Flagged = 0, Text = "I am Batman!",
            PubDate = DateTime.UtcNow.AddMinutes(1).Ticks
        };
        var chip = new Message
        {
            Author = bill, AuthorId = 3, Flagged = 0, Text = "Get microsoft chip. Very good, very niice",
            PubDate = DateTime.UtcNow.AddDays(2).Ticks
        };
        context.Messages.AddRange(hello, bye, batman, chip);
        context.Followers.Add(new Follower {WhoId = 2, WhomId = 1});

        context.SaveChanges();

        _context = context;
        _minitwit = new MinitwitReact.Minitwit(context);
    }
    [Theory]
    [InlineData("Elon Musk", 1)]
    [InlineData("Jeff Bezos", 2)]
    [InlineData("Bill Gates",3)]
    [InlineData("Bruce Wayne",4)]
    public async Task GetUserId_returns_UserId_given_valid_username(string username, long expected)
    {
        Assert.Equal(expected, await _minitwit.GetUserId(username));
    }
    [Fact]
    public async Task GetUserIdEF_returns_0_given_invalid_username()
    {
        Assert.Equal(0, await _minitwit.GetUserId("Irrelevant person"));
    }
    [Fact]
    public async Task GetUserDetailsById_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _minitwit.GetUserDetialsById(1);
        Assert.Equal(elon.Username, actual.Username);
        Assert.Equal(elon.Email, actual.Email);
        Assert.Equal(elon.UserId, actual.UserId);
        Assert.Equal(elon.PwHash, actual.PwHash);
    }
    [Fact]
    public async Task GetUsersEf_returns_Users()
    {
        var actual = await _minitwit.GetAllUsers();
        Assert.Equal("Elon Musk", actual.First().Username);
    }
    [Fact]
    public async Task PublicTimeline_returns_public_timeline()
    {
        var actual = await _minitwit.PublicTimeline();
        var valueTuples = actual.ToList();
        
        Assert.Equal("Jeff Bezos", valueTuples.First().Item2.Username);
        Assert.Equal("Elon bad", valueTuples.First().Item1.Text);
        Assert.Equal("Bruce Wayne", valueTuples.Last().Item2.Username);
        Assert.Equal("I am Batman!", valueTuples.Last().Item1.Text);
    }

    [Fact]
    public async Task PostMessage_Creates_new_message_by_user()
    {
        var response = await _minitwit.PostMessageEf(1, "I make a new post yes");
        Assert.Equal(Status.Created, response);
    }

    [Fact]
    public async Task PostMessage_returns_NotFound_given_invalid_userid()
    {
        var response = await _minitwit.PostMessageEf(111, "I make a new post yes");
        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task PostMessage_Posts_Message_to_top_of_timeline()
    {
        var response = await _minitwit.PostMessageEf(1, "I make a new post yes");
        Assert.Equal(Status.Created, response);

        var timeline = await _minitwit.PublicTimeline();
        timeline = timeline.ToList();
        //Post by user 1?
        Assert.Equal(1, timeline.Last().Item2.UserId);
        //Post correct post?
        Assert.Equal("I make a new post yes", timeline.Last().Item1.Text);
    }

    [Fact]
    public async Task FollowUser_returns_updated_given_valid_session_and_target()
    {
        var response = await _minitwit.FollowUserEf(1, "Jeff Bezos");
        Assert.Equal(Status.Updated, response);
    }

    [Fact]
    public async Task FollowUser_returns_NotFound_given_invalid_session_and_target()
    {
        var response = await _minitwit.FollowUserEf(0, "Jeffers");
        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task FollowUser_returns_Conflict_given_existing_follow()
    {
        var response = await _minitwit.FollowUserEf(2, "Elon Musk");
        Assert.Equal(Status.Conflict, response);
    }

    [Fact]
    public async Task FollowUser_add_follow_to_database()
    {
        var response = await _minitwit.FollowUserEf(1, "Jeff Bezos");
        var jeff = new UserDto( 2, "Jeff Bezos");
        var follows = await _minitwit.Follows(1, jeff);
        Assert.True(follows);
    }

    [Fact]
    public async Task UnFollowUser_returns_updated_given_valid_session_and_target()
    {
        var response = await _minitwit.UnfollowUserEf(2, "Elon Musk");
        Assert.Equal(Status.Updated, response);
    }
    [Fact]
    public async Task UnFollowUser_returns_Notfound_given_invalid_session_and_target()
    {
        var response = await _minitwit.UnfollowUserEf(0, "Elonis");
        Assert.Equal(Status.NotFound, response);
    }
    [Fact]
    public async Task UnFollowUser_returns_Conflict_given_nonExisting_follow()
    {
        var response = await _minitwit.UnfollowUserEf(1, "Jeff Bezos");
        Assert.Equal(Status.Conflict, response);
    }
    [Fact]
    public async Task UnfollowUser_removes_follow_to_database()
    {
        var elon = new UserDto(1, "Elon Musk");
        var follows = await _minitwit.Follows(2, elon);
        Assert.True(follows);
        
        var response = await _minitwit.UnfollowUserEf(2, "Elon Musk");
        var unfollows = await _minitwit.Follows(2, elon);
        Assert.False(unfollows);
    }

    [Fact]
    public async Task Login_given_valid_username_and_pw_returns_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var login = await _minitwit.LoginEf(elon.Username, elon.PwHash);
        Assert.Equal(1, login);
    }

    [Fact]
    public async Task Login_given_invalid_username_and_pw_returns_0()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var login = await _minitwit.LoginEf("elonis", elon.PwHash);
        Assert.Equal(0, login);
    }
    [Fact]
    public async Task Login_given_valid_username_and_invalid_pw_returns_minus1()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var login = await _minitwit.LoginEf(elon.Username, "Tesla->Moon");
        Assert.Equal(-1, login);
    }

    [Fact]
    public async Task register_returns_id_given_nonExisting_user()
    {
        var register = await _minitwit.RegisterEf("New User", "user@user.com", "SafePassword");
        Assert.Equal(5, register);
    }
    [Fact]
    public async Task register_returns_0_given_Existing_user()
    {
        var register = await _minitwit.RegisterEf("Elon Musk", "user@user.com", "SafePassword");
        Assert.Equal(0, register);
    }
    
    [Fact]
    public async Task GetUserById_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _minitwit.GetUserById(1);
        Assert.Equal(elon.Username, actual.Username);
        Assert.Equal(elon.UserId, actual.UserId);
    }

    // TEST FOR ADD MESSAGES
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing:true);
        GC.SuppressFinalize(this);
    }
}