using Microsoft.Data.Sqlite;

namespace Minitwit;

public class Minitwit
{
// configuration
    const string DATABASE = "Data Source=./minitwit.db";
    const int PER_PAGE = 30;
    const bool DEBUG = true;
    const string SECRET_KEY = "development key";
    
    
    public async Task Connect_db()
    {
        var connection = new SqliteConnection(DATABASE);
        await connection.OpenAsync();
    }
}