

namespace Minitwit.Tests;


public class MinitwitTests : IDisposable, IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MinitwitTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        // Create an httpclient for api tests
        _client = _factory.CreateClient();
    }



    // API TESTS
    [Fact]
    public async Task HTTP_GET_Users_Success(){
        // await using var app = new WebApplicationFactory<Program>();
        // using var _client = app.CreateClient();
        var response = await _client.GetAsync("minitwit/Users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HTTP_GET_Msgs_Success(){
        var response = await _client.GetAsync("minitwit/msgs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HTTP_GET_Timeline_Success(){
        var response = await _client.GetAsync("minitwit/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task HTTP_GET_UserTimeline_Success(){
        var response = await _client.GetAsync("minitwit/1/Jeff Bezos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task HTTP_POST_Follow_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/follow/1/Jeff Bezos", "");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task HTTP_POST_Unfollow_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/unfollow/2/Elon Musk", "");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task HTTP_GET_Login_Success(){
        var response = await _client.GetAsync("minitwit/login/Elon Musk/123");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task HTTP_POST_Message_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/1/some message", "");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task HTTP_POST_Register_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/apiTestUsername/apitest@email.com/yeet","");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // TEST FOR ADD MESSAGES
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // _context.Dispose();
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