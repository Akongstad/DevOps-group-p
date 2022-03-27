
using MinitwitReact.Core;

namespace Minitwit.IntegrationTests;

public class MinitwitTests : IDisposable, IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MinitwitTests(CustomWebApplicationFactory factory)
    {
        // Create an httpclient for api tests
        _client = factory.CreateClient();
    }



    // API TESTS
    [Fact]
    public async Task HTTP_GET_Users_Success(){
        // await using var app = new WebApplicationFactory<Program>();
        // using var _client = app.CreateClient();
        var response = await _client.GetAsync("minitwit/Users");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_GET_Msgs_Success(){
        var response = await _client.GetAsync("minitwit/msgs");
        response.Should().BeSuccessful();
    }

    //fails if the route is minitwit/msgs/{id} - changed in controller
    [Fact]
    public async Task HTTP_GET_Timeline_Success(){
        var response = await _client.GetAsync("minitwit/msgs/Jeff Bezos/");
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_GET_UserTimeline_Success(){
        var response = await _client.GetAsync("minitwit/msgs/Jeff Bezos");
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_POST_Follow_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/follow", new FollowerDto(1, "Jeff Bezos"));
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_POST_Unfollow_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/unfollow", new FollowerDto(2, "Elon Musk"));
        response.Should().BeSuccessful();
    }
    
    /*[Fact]
    public async Task HTTP_GET_Login_Success(){
        var response = await _client.GetAsync("minitwit/login");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }*/
    
    [Fact]
    public async Task HTTP_POST_Message_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/msg/1", new MessageCreateDto() {Text = "some message", PubDate = 2022});
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_POST_Register_Success(){
        var response = await _client.PostAsJsonAsync("minitwit/register",new UserCreateDto() {Username = "apiTestUsername", Email = "apitest@email.com", PwHash = "yeet"});
        response.Should().BeSuccessful();
    }






    // API TESTS Simulation
    [Fact]
    public async Task HTTP_GET_Latest_Simulation(){
        var response = await _client.GetAsync("minitwitSimulation/latest");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_GET_Msgs_Simulation(){
        var response = await _client.GetAsync("minitwitSimulation/msgs");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_GET_Timeline_Simulation(){
        var response = await _client.GetAsync("minitwitSimulation/msgs/Jeff Bezos/");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Message_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/msgs/Jeff Bezos/", new {content = "some message"});
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_GET_Follows_Simulation(){
        var response = await _client.GetAsync("minitwitSimulation/fllws/Jeff Bezos/");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Follow_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/fllws/Jeff Bezos/", new {follow = "Elon Musk"});
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_UnFollow_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/fllws/Jeff Bezos/", new {unfollow = "Elon Musk"});
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Register_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/register/", new {username = "testusername", email = "test@email.com", pwd = "testpass"});
        response.Should().BeSuccessful();
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