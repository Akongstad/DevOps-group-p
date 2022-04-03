// Missing Login success!

namespace Minitwit.Tests.ControllerTests;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _logger;
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<IJwtUtils> _token;

    public UserControllerTests()
    {
        _logger = new Mock<ILogger<UserController>>();
        _repository = new Mock<IUserRepository>();
        _token = new Mock<IJwtUtils>();
    }
    /*[Fact]
    public async Task PostRegister_creates_New_user_only_returns_long()
    {
        // Arrange
        var toCreate = new UserCreateDto();
        const int created = 1;
        _repository.Setup(m => m.Register(toCreate)).ReturnsAsync(created);
        var controller = new UserController(_logger.Object, _repository.Object, _token.Object);
        
        // Act
        var result = await controller.PostRegister(toCreate);

        // Assert
        Assert.Equal(created, result);
    }
    
    [Fact]
    public async Task GetAll_returns_All_Users_from_repo() {
        // Arrange
        var expected = Array.Empty<UserDetailsDto>();
        _repository.Setup(m => m.GetAllUsers()).ReturnsAsync(expected);
        var controller = new UserController(_logger.Object, _repository.Object, _token.Object);

        // Act
        var actual = await controller.Get();

        // Assert
        Assert.Equal(expected, actual);
    }*/
    
    /*[Fact]
    public async Task PostLogin_returns_UserLoginResponseDto_and_token()
    {
        // Arrange
        var toCreate = new UserLoginDto("Elon", "musks");
        const long created = 1;
        

        _repository.Setup(m => m.Login(toCreate.Username, toCreate.PwHash)).ReturnsAsync(created).Verifiable();
        var controller = new UserController(_logger.Object, _repository.Object, _token.Object);
        
        // Act
        var result = await controller.Login(toCreate) as CreatedResult;

        // Assert
        Assert.Equal( 201, result?.StatusCode);
        //Assert.Equal("Post", result?.ActionName);

    }
  
    [Fact]
    public async Task PostRegister_if_user_is_null_return_0()
    {
        // Arrange
        var toCreate = new UserCreateDto();
        _repository.Setup(m => m.Register(toCreate.Username, toCreate.Email, toCreate.PwHash)).ReturnsAsync(null);
        var controller = new UserController(_logger.Object, _repository.Object, _token.Object);
        
        // Act
        var result = await controller.PostRegister(null!);

        // Assert
        Assert.Equal(0, result);
    } */
    
}