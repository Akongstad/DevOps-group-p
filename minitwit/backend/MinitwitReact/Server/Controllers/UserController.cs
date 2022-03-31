namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger; //??
    private readonly IUserRepository _userRepository;
    private readonly IJwtUtils _jwtUtils;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository, IJwtUtils jwtUtils)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtUtils = jwtUtils;
    }
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<long> PostRegister ([FromBody] UserCreateDto user)
    {
        return await _userRepository.Register(user.Username, user.Email, user.PwHash);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto login)
    {
        var user = await  _userRepository.GetUserDetailsByName(login.Username);
        if (user is null) return BadRequest(new {message = "Invalid Username"});
        
        var id = await _userRepository.Login(user.Username, login.PwHash);
        if(id < 1) return BadRequest(new {message = "Invalid Password"});

        var jwt = _jwtUtils.GenerateToken(user);
        
        return Ok(new UserLoginResponseDto(user.UserId, user.Username, user.Email, jwt));
    }
    
    
    [HttpGet("")]
    public Task<IEnumerable<UserDto>> Get(){
        return _userRepository.GetAllUsers();
    }

}