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
    public void Test_if_tempfile_with_schema_created()
    {
        var actual = _minitwit.GetUsers();
        Assert.NotNull(_minitwit);
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
        GC.SuppressFinalize(this);
    }
}