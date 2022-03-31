namespace Minitwit.IntegrationTests;

public class MinitwitSimulationIntegrationTests : IDisposable, IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MinitwitSimulationIntegrationTests(CustomWebApplicationFactory factory)
    {
        // Create an httpclient for api tests
        _client = factory.CreateClient();
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
        var response = await _client.GetAsync("minitwitSimulation/msgs/Elon Musk");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Message_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/msgs/Jeff Bezos", new {content = "some message"});
        response.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task HTTP_GET_Follows_Simulation(){
        var response = await _client.GetAsync("minitwitSimulation/fllws/Jeff Bezos");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Follow_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/fllws/Jeff Bezos", new {follow = "Elon Musk"});
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_UnFollow_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/fllws/Jeff Bezos", new {unfollow = "Elon Musk"});
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task HTTP_POST_Register_Simulation(){
        var response = await _client.PostAsJsonAsync("minitwitSimulation/register", new {username = "testusername", email = "test@email.com", pwd = "testpass"});
        response.Should().BeSuccessful();
    }
    
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing:true);
        GC.SuppressFinalize(this);
    }
    
}