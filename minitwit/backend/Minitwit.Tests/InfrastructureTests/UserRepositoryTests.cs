using MinitwitReact.Core.Enums;

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
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _userRepository.GetUserDetailsById(1);
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.Email, actual.Email);
        Assert.Equal(elon.UserId, actual.UserId);
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
        var user = new User
            {Username = "Hash Tester", Email = "Hash@live.com", PwHash = "hashed"};
        
        var login = await _userRepository.Login(new UserLoginDto(user.Username, user.PwHash));
        Assert.Equal(AuthStatus.Authorized, login);
    }

    [Fact]
    public async Task Login_given_invalid_username_AuthStatus_WrongUsername()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var login = await _userRepository.Login(new UserLoginDto("elonis", elon.PwHash));
        Assert.Equal(AuthStatus.WrongUsername, login);
    }
    [Fact]
    public async Task Login_given_valid_username_and_invalid_pw_returns_AuthStatus_WrongPassword()
    {
        var login = await _userRepository.Login(new UserLoginDto("Hash Tester", "Tesla->Moon"));
        Assert.Equal(AuthStatus.WrongPassword, login);
    }

    [Fact]
    public async Task register_returns_Success()
    {
        var register = await _userRepository.Register(new UserCreateDto() {Username = "New User", Email = "user@user.com", PwHash = "SafePassword"});
        Assert.Equal(AuthStatus.Authorized, register);
    }
    [Fact]
    public async Task register_returns_0_given_Existing_user()
    {
        var register = await _userRepository.Register(new UserCreateDto() {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "SafePassword"});
        Assert.Equal(AuthStatus.UsernameInUse, register);
    }

    [Fact]
    public async Task GetUserById_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _userRepository.GetUserById(1);
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.UserId, actual.UserId);
    }
    
    [Fact]
    public async Task GetUserDetailsByName_returns_User_given_valid_id()
    {
        var elon = new User {Username = "Elon Musk", Email = "Tesla@gmail.com", PwHash = "123", UserId = 1};
        var actual = await _userRepository.GetUserDetailsByName("Elon Musk");
        Assert.Equal(elon.Username, actual!.Username);
        Assert.Equal(elon.UserId, actual.UserId);
    }
    
    
}