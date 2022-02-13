using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MinitwitReact.Core;
using MinitwitReact.Entities;


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

    private readonly SqliteConnection _connection;
    private readonly IMinitwitContext _context;
    
    public Minitwit(IMinitwitContext context)
    {
        _context = context;
        _connection = ConnectDb();
    }
    public Minitwit(IMinitwitContext context ,SqliteConnection connection)
    {
        _context = context;
        _connection = connection;
    }

    public SqliteConnection ConnectDb()
    {
        using var connection = new SqliteConnection(DATABASE);
        return connection;
    }
    
    public void InitDb()
    {
        var cmd = File.OpenText("./../../../../../schema.sql").ReadToEnd();
        using var command = new SqliteCommand(cmd, _connection);
        
        OpenConnection();
        var changed = command.ExecuteNonQuery();
        Console.WriteLine("rows affected: " + changed);
        CloseConnection();
    }

    //___________________EF Core implementations________________________________//
    public async Task<IEnumerable<User>> GetUsersEf() => await _context.Users.Select(u => u).ToListAsync();

    public async Task<User?> GetUserEf(long userid) =>
        await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid);

    public async Task<long> GetUserIdEf(string username)
    {
       var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
       if (user == null)
       {
           return 0;
       }
       return user.UserId;
    }

    public async Task<IEnumerable<ValueTuple<Message,User>>> PublicTimelineEf()
    {
        var timeline = from m in _context.Messages
            join u in _context.Users on m.AuthorId equals u.UserId
            where m.Flagged == 0
            orderby m.PubDate descending
            select new {m, u};
        var reformat = timeline.Select(i => new ValueTuple<Message, User>(i.m, i.u));
        return await reformat.ToListAsync();
    }

    public async Task<bool> Follows(long sessionId, User user)
    {
        var follows = await _context.Followers.Where(f => f.WhoId == sessionId && f.WhomId == user.UserId).ToListAsync();
        return follows.Count > 0;
    }

    public async Task<IEnumerable<ValueTuple<Message, User>>> UserTimelineEf(long sessionId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return null!;
        }
        var follows = await Follows(sessionId, user);
        if (!follows)
        {
            return await PublicTimelineEf();
        }
        var timeline = from m in _context.Messages
            join u in _context.Users on m.AuthorId equals u.UserId
            where u.UserId == m.AuthorId
            where u.UserId == user.UserId
            orderby m.PubDate descending
            select new {m, u};
        var reformat = timeline.Select(i => new ValueTuple<Message, User>(i.m, i.u));
        return await reformat.ToListAsync();
    }

    public async Task<IEnumerable<ValueTuple<Message, User>>> TimelineEf(long sessionId)
    {
        var user = await GetUserEf(sessionId);
        if (sessionId > 0 && user != null)
        {
            return await UserTimelineEf(sessionId, user.Username);
        }

        return await PublicTimelineEf();
    }

    //_____________________Raw Sql implementation__________________________________________
    public IEnumerable<string> GetUsers()
    {
        var cmd = "SELECT username FROM user";
        using var command = new SqliteCommand(cmd, _connection);
        OpenConnection();

        using var reader = command.ExecuteReader();

        

        while (reader.Read())
        {
            yield return reader.GetString("username");
        }
        CloseConnection();
    }

    // Queries the database and returns..
    // other type for
    public ICollection<Dictionary<string, string>> QueryDb(SqliteCommand query, bool one=false)
    {
        OpenConnection();
        using var reader = query.ExecuteReader();

        // List<Dictionary<string,string>> result = new List<Dictionary<string,string>>();
        // while (reader.Read())
        // {
        //     var dict = new Dictionary<string, string>();
        //     for (int i=0; i < reader.FieldCount; i++){
        //         dict.Add(reader.GetName(i), reader[i].ToString());
        //     }; 
        //     result.Add(dict);
        // };
        
        CloseConnection();
        //return result.DefaultIfEmpty() as ICollection<Dictionary<string, string>>;
        return null!;
    }

    // Convenience method to look up the id for a username.
    public long GetUserId(string username)
    {
        OpenConnection();
        var cmd = "select user_id from user where username = @username";
        using var command = new SqliteCommand(cmd, _connection);
        
        command.Parameters.AddWithValue("@username", username);
        using var reader = command.ExecuteReader();
        
        reader.Read();
        var response = reader.GetInt32("user_id");
        CloseConnection();
        return response;
    }

    public User GetUser(long userid){

        var cmd = "select * from user where user_id = @user_id";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@user_id", userid);
        
        OpenConnection();
        using var reader = command.ExecuteReader();
        reader.Read();
        var user = new User {
            Username = reader.GetString("username"),
            UserId = reader.GetInt64("user_id"),
            Email = reader.GetString("email"),
            PwHash = reader.GetString("pw_hash")
        };
        CloseConnection();
        return user;
    }
    
    public bool isFollow(long sessionId, string username){
        var profileId = GetUserId(username);
        var cmd = "select 1 from follower where follower.who_id = @session_userid and follower.whom_id = @profile_userid";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@profile_userid", profileId);
        command.Parameters.AddWithValue("@session_userid", sessionId);
        
        OpenConnection();
        using var reader = command.ExecuteReader();
        reader.Read();
        CloseConnection();
        return reader.HasRows;
    }

    // Format a timestamp for display.
    public DateTime FormatDatetime(string timestamp)
    {
        return DateTime.Parse(timestamp);
        //.utcfromtimestamp(timestamp).strftime('%Y-%m-%d @ %H:%M')
    }
    // TODO: Make a Datetime convert from Datetime.Now to string

    // Return the gravatar image for the given email address.
    public Uri gravatar_url(string email, int size = 80)
    {
        var emailTrim = email.ToLower().Trim();
        return new Uri($"http://www.gravatar.com/avatar/{email}?d=identicon&s={size}");
    }

    // Make sure we are connected to the database each request and look
    // up the current user so that we know they are there.

    public void before_request()
    {
        throw new NotImplementedException();
    }

    public void after_request(string response)
    {
        throw new NotImplementedException();
    }

    /* Shows a users timeline or if no user is logged in it will
    redirect to the public timeline. This timeline shows the user's
    messages as well as all the messages of followed users. */
    public IEnumerable<(Message, User)> Timeline(long userid)
    {
        // request.remote_addr must be the given IP-address?
        // redirect to public_timeline if g.user (if user is not logged in) is not satisfied
        
        var cmd =
            "select message.*, user.* from message, user\n where message.flagged = 0 and message.author_id = user.user_id and (\nuser.user_id = @user_id or \n user.user_id in (select whom_id from follower\n where who_id = @user_id))\norder by meesage.pub_date desc limit @limit";
        using var command = new SqliteCommand(cmd, _connection);
        
        OpenConnection();
        command.Parameters.AddWithValue("@limit", PER_PAGE);
        command.Parameters.AddWithValue("@user_id", userid);
        using var reader = command.ExecuteReader();
        
        var messages = new List<(string,string)>();
        while (reader.Read())
        {
            var message = new Message
            {
                MessageId = reader.GetInt64("message_id"),
                AuthorId = reader.GetInt64("author_id"),
                Flagged = reader.GetInt64("flagged"),
                PubDate = reader.GetInt64("pub date"),
                Text = reader.GetString("text")
            };
             var user = new User
            {
                Username = reader.GetString("username"),
                UserId = reader.GetInt64("user_id"),
                Email = reader.GetString("email")
            };
            yield return (message, user);
        }           
        CloseConnection();
    }

    public IEnumerable<(Message, User)> public_timeline()
    {
        var cmd =
            @"select message.*, user.* from message, user 
            where message.flagged = 0 and message.author_id = user.user_id
            order by message.pub_date desc limit @limit";
        using var command = new SqliteCommand(cmd, _connection);
        OpenConnection();

        command.Parameters.AddWithValue("@limit", PER_PAGE);
        using var reader = command.ExecuteReader();


        while (reader.Read())
        {
            var message = new Message
            {
                MessageId = reader.GetInt64("message_id"),
                AuthorId = reader.GetInt64("author_id"),
                Flagged = reader.GetInt64("flagged"),
                PubDate = reader.GetInt64("pub_date"),
                Text = reader.GetString("text")
            };
            var user = new User
            {
                Username = reader.GetString("username"),
                UserId = reader.GetInt64("user_id"),
                Email = reader.GetString("email"),
                PwHash = reader.GetString("pw_hash")
            };
            yield return (message, user);
        }
        CloseConnection();
    }

    public IEnumerable<(Message, User)> user_timeline(long sessionId ,string username)
    {
    
        var profileid = GetUserId(username);
        
        var cmd = @"select message.*, user.* from message, user where user.user_id = message.author_id and user.user_id = @profile_userid
        order by message.pub_date desc limit @limit";
    
        using var command2 = new SqliteCommand(cmd, _connection);
        command2.Parameters.AddWithValue("@profile_userid", profileid);
        command2.Parameters.AddWithValue("@limit", PER_PAGE);
        
        using var reader = command2.ExecuteReader();
        while (reader.Read())
        {
            var message = new Message
            {
                MessageId = reader.GetInt64("message_id"),
                AuthorId = reader.GetInt64("author_id"),
                Flagged = reader.GetInt64("flagged"),
                PubDate = reader.GetInt64("pub_date"),
                Text = reader.GetString("text")
            };
            yield return (message, GetUser(profileid));
        }
        CloseConnection();
    }


    public long follow_user(string username, long userid)
    {
        long followid = GetUserId(username);
        if (followid < 1) {
            throw new ArgumentNullException();
        }

        var cmd = "insert into follower (who_id, whom_id) values (@user_id, @follow_id)";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@user_id", userid);
        command.Parameters.AddWithValue("@follow_id", followid);

        OpenConnection();
    
        using var reader = command.ExecuteReader();
        reader.Read();
        CloseConnection();
        return followid;
    }

    public long unfollow_user(string username, long userid)
    {
        long followid = GetUserId(username);
        if (followid < 1) {
            throw new ArgumentNullException();
        }

        var cmd = "delete from follower where who_id=@user_id and whom_id=@follow_id";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@user_id", userid);
        command.Parameters.AddWithValue("@follow_id", followid);
        
        OpenConnection();
        
        using var reader = command.ExecuteReader();
        reader.Read();
        
        CloseConnection();
        
        return followid;
    }

    public string add_message(long userid, string message)
    {
        
        var cmd = "insert into message (@author_id, @text, @pub_date, flagged) values (?, ?, ?, 0)";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@author_id", userid);
        command.Parameters.AddWithValue("@text", message);
        command.Parameters.AddWithValue("@pub_date", System.DateTime.Now.ToString());

        OpenConnection();
    
        using var reader = command.ExecuteReader();
        reader.Read();
        CloseConnection();
        return message;
    }

    public long Login(string username, string pw)
    {
        var cmd = "select user_id from user where username = @username and pw_hash = @pw_hash";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@pw_hash", pw);

        OpenConnection();    
        using var reader = command.ExecuteReader();
        reader.Read();
        var result = reader.GetInt64("user_id");
        CloseConnection();
        return reader.GetInt64("user_id");
    }

    public long Register(string username, string email, string pw)
    {
        var cmd = "insert into user ( username, email, pw_hash) values (@username, @email, @pw_hash)";
        using var command = new SqliteCommand(cmd, _connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@pw_hash", pw);

        OpenConnection();
    
        using var reader = command.ExecuteReader();
        reader.Read();
        CloseConnection();
        return GetUserId(username);
    }

    public void Logout(long userid)
    {
        throw new NotImplementedException();
    }

    private void OpenConnection()
    {
        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }
    }

    private void CloseConnection()
    {
        if (_connection.State == ConnectionState.Open)
        {
            _connection.Close();
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}