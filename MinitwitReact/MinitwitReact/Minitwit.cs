using System.Data;

namespace MinitwitReact;

using Microsoft.Data.Sqlite;

public class Minitwit : IMinitwit, IDisposable
{
// configuration
    const string DATABASE = "Data Source=./../../../../../minitwit.db";
    const int PER_PAGE = 30;
    const bool DEBUG = true;
    const string SECRET_KEY = "development key";

    private readonly SqliteConnection _connection;
    
    public Minitwit()
    {
        _connection = ConnectDb();
    }
    public Minitwit(SqliteConnection connection)
    {
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

    public DataTable GetSchema()
    {
        return _connection.GetSchema();
    }

    public IEnumerable<string> GetUsers()
    {
        var cmd = "SELECT username FROM user";
        using var command = new SqliteCommand(cmd, _connection);
        OpenConnection();

        using var reader = command.ExecuteReader();

        var users = new List<string>();
        while (reader.Read())
        {
            yield return reader.GetString("username");
        }
        CloseConnection();
        
    }

    public void QueryDb()
    {
        
    }

    public void GetUserId(string username)
    {
        throw new NotImplementedException();
    }

    public void FormatDatetime(string timestamp)
    {
        throw new NotImplementedException();
    }

    public void gravatar_url(string email, int size = 80)
    {
        throw new NotImplementedException();
    }

    public void before_request()
    {
        throw new NotImplementedException();
    }

    public void after_request(string response)
    {
        throw new NotImplementedException();
    }

    public void Timeline()
    {
        throw new NotImplementedException();
    }

    public void public_timeline()
    {
        throw new NotImplementedException();
    }

    public void user_timeline(string username)
    {
        throw new NotImplementedException();
    }

    public void follow_user(string username)
    {
        throw new NotImplementedException();
    }

    public void unfollow_user(string username)
    {
        throw new NotImplementedException();
    }

    public void add_message()
    {
        throw new NotImplementedException();
    }

    public void Login()
    {
        throw new NotImplementedException();
    }

    public void Register()
    {
        throw new NotImplementedException();
    }

    public void Logout()
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