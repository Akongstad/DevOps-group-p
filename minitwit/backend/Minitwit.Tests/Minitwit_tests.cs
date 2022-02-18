<<<<<<< HEAD
using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using MinitwitReact;
using Xunit;

=======
>>>>>>> main
namespace Minitwit.Tests;

public class MinitwitTests : IDisposable
{
    private readonly IMinitwit _minitwit;
<<<<<<< HEAD

    public MinitwitTests()
    {
        var tempfile = Path.GetTempFileName();
        var connection = new SqliteConnection($"Data source={tempfile}");
        connection.Open();
        _minitwit = new MinitwitReact.Minitwit(connection);
        _minitwit.InitDb();
    }

=======
    //EF core
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
            PubDate = DateTime.UtcNow.AddDays(3).ToBinary()
        };
        var bye = new Message
        {
            Author = elon, AuthorId = 1, Flagged = 0, Text = "Tesla stonks",
            PubDate = DateTime.UtcNow.AddDays(1).ToBinary()
        };
        var batman = new Message
        {
            Author = bruce, AuthorId = 4, Flagged = 0, Text = "I am Batman!",
            PubDate = DateTime.UtcNow.ToBinary()
        };
        var chip = new Message
        {
            Author = bill, AuthorId = 3, Flagged = 0, Text = "Get microsoft chip. Very good, very niice",
            PubDate = DateTime.UtcNow.AddDays(2).ToBinary()
        };
        context.Messages.AddRange(hello, bye, batman, chip);
        context.SaveChanges();

        _context = context;

        //Setup for raw sql
        var tempfile = Path.GetTempFileName();
        var connection = new SqliteConnection($"Data source={tempfile}");
        connection.Open();
        _minitwit = new MinitwitReact.Minitwit(context, connection);
        //_minitwit.InitDb();
    }
>>>>>>> main
    [Fact]
    public void Test_if_tempfile_with_schema_created()
    {
        var actual = _minitwit.GetUsers();
        Assert.NotNull(_minitwit);
<<<<<<< HEAD
        Assert.NotNull(actual);
        Assert.NotEqual(0,_minitwit.GetSchema().Rows.Count);
    }

    [Fact]
    public void GetUsername_returns_UserName()
    {
        var actual = _minitwit.GetUserId("Roger Histand");
        var actual1 = _minitwit.GetUserId("Geoffrey Stieff");
        var actual2 = _minitwit.GetUserId("Wendell Ballan");
        var actual3 = _minitwit.GetUserId("Nathan Sirmon");
        Assert.Equal(1, actual);
        Assert.Equal(2, actual1);
        Assert.Equal(3, actual2);
        Assert.Equal(4, actual3);
    }
    // TEST FOR ADD MESSAGES

    public void Dispose()
    {
=======
        Assert.NotNull(actual); 
    }
    [Fact]
    public async Task GetUserId_returns_UserId_given_valid_username()
    {
        Assert.Equal(1, await _minitwit.GetUserIdEf("Elon Musk"));
        Assert.Equal(2, await _minitwit.GetUserIdEf("Jeff Bezos"));
        Assert.Equal(3, await _minitwit.GetUserIdEf("Bill Gates"));
        Assert.Equal(4, await _minitwit.GetUserIdEf("Bruce Wayne"));
    }
    [Fact]
    public async Task GetUserId_returns_0_given_invalid_username()
    {
        Assert.Equal(0, await _minitwit.GetUserIdEf("Irrelevant person"));
    }
    [Fact]
    public async Task GetUserEF_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _minitwit.GetUserEf(1);
        Assert.Equal(elon.Username, actual.Username);
        Assert.Equal(elon.Email, actual.Email);
        Assert.Equal(elon.UserId, actual.UserId);
        Assert.Equal(elon.PwHash, actual.PwHash);
    }
    [Fact]
    public async Task GetUsersEf_returns_Users()
    {
        var actual = await _minitwit.GetUsersEf();
        Assert.Equal("Elon Musk", actual.First().Username);
    }
    [Fact]
    public async Task PublicTimeline_returns_public_timeline()
    {
        var actual = await _minitwit.PublicTimelineEf();
        var valueTuples = actual.ToList();
        
        Assert.Equal("Jeff Bezos", valueTuples.First().Item2.Username);
        Assert.Equal("Elon bad", valueTuples.First().Item1.Text);
        Assert.Equal("Bill Gates", valueTuples[1].Item2.Username);
        Assert.Equal("Get microsoft chip. Very good, very niice", valueTuples[1].Item1.Text);
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
>>>>>>> main
        GC.SuppressFinalize(this);
    }
}