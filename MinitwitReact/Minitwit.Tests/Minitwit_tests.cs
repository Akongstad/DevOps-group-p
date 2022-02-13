using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using MinitwitReact;
using Xunit;

namespace Minitwit.Tests;

public class MinitwitTests : IDisposable
{
    private readonly IMinitwit _minitwit;

    public MinitwitTests()
    {
        var tempfile = Path.GetTempFileName();
        var connection = new SqliteConnection($"Data source={tempfile}");
        connection.Open();
        _minitwit = new MinitwitReact.Minitwit(connection);
        _minitwit.InitDb();
    }

    [Fact]
    public void Test1()
    {
        var expected = "Hello";
        var actual = _minitwit.GetUsers();
        Assert.NotNull(_minitwit);
        Assert.NotNull(actual);
        Assert.NotNull(_minitwit.GetSchema().Rows.Count);
    }
  
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}