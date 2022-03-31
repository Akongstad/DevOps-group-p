namespace Minitwit.Tests.InfrastructureTests;

public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new UserRepository(Context);
    }
    
    [Theory]
    [InlineData("Elon Musk", 1)]
    [InlineData("Jeff Bezos", 2)]
    [InlineData("Bill Gates",3)]
    [InlineData("Bruce Wayne",4)]
    public async Task GetUserId_returns_UserId_given_valid_username(string username, long expected)
    {
        Assert.Equal(expected, await _userRepository.GetUserIdFromUsername(username));
    }
    [Fact]
    public async Task GetUserIdEF_returns_0_given_invalid_username()
    {
        Assert.Equal(0, await _userRepository.GetUserIdFromUsername("Irrelevant person"));
    }
    
    [Fact]
    public async Task GetUserDetailsById_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", Id = 1};
        var actual = await _userRepository.GetUserDetailsById(1);
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.Email, actual.Email);
        Assert.Equal(elon.Id, actual.UserId);
        Assert.Equal(elon.PwHash, actual.PwHash);
    }
    [Fact]
    public async Task GetUsersEf_returns_Users()
    {
        var actual = await _userRepository.GetAllUsers();
        Assert.Equal("Elon Musk", actual.First().Username);
    }
    
    [Fact]
    public async Task Login_given_valid_username_and_pw_returns_id()
    {
        var hashman = new User
            {Username = "Hash Tester", Email = "Hash@live.com", PwHash = "hashed"};
        
        var login = await _userRepository.Login(hashman.Username, hashman.PwHash);
        Assert.Equal(5, login);
    }

    [Fact]
    public async Task Login_given_invalid_username_and_pw_returns_0()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", Id = 1};
        var login = await _userRepository.Login("elonis", elon.PwHash);
        Assert.Equal(0, login);
    }
    [Fact]
    public async Task Login_given_valid_username_and_invalid_pw_returns_minus1()
    {
        var login = await _userRepository.Login("Hash Tester", "Tesla->Moon");
        Assert.Equal(-1, login);
    }

    [Fact]
    public async Task register_returns_id_given_nonExisting_user()
    {
        var register = await _userRepository.Register("New User", "user@user.com", "SafePassword");
        Assert.Equal(6, register);
    }
    [Fact]
    public async Task register_returns_0_given_Existing_user()
    {
        var register = await _userRepository.Register("Elon Musk", "Tesla@gmail.com", "SafePassword");
        Assert.Equal(0, register);
    }
    
    [Fact]
    public async Task register_returns_0_given_Existing_use()
    {
        var register = await _userRepository.Register("Elon Musk", "Tesla@gmail.com", "SafePassword");
        Assert.Equal(0, register);
    }
    
    [Fact]
    public async Task GetUserById_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", Id = 1};
        var actual = await _userRepository.GetUserById(1);
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.Id, actual.UserId);
    }
    
    [Fact]
    public async Task GetUserDetailsByName_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", Id = 1};
        var actual = await _userRepository.GetUserDetailsByName("Elon Musk");
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.Id, actual.UserId);
    }
    
    
}