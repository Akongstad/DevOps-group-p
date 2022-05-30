namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtUtils _jwtUtils;

    public UserController(IUserRepository userRepository, IJwtUtils jwtUtils)
    {
        _userRepository = userRepository;
        _jwtUtils = jwtUtils;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostRegister ([FromBody] UserCreateDto user)
    {
        var result = await _userRepository.Register(user);
        if (result == AuthStatus.UsernameInUse)
        {
            return BadRequest(new {message = "Name already in use"});
        }

        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto login)
    {
        var result = await _userRepository.Login(login);
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (result == AuthStatus.WrongUsername)
        {
            return BadRequest(new {message = "Invalid Username"});
        }
        if (result == AuthStatus.WrongPassword)
        {
            return BadRequest(new {message = "Invalid Password"});
        }

        var user = _userRepository.GetUserDetailsByName(login.Username).Result;
        if (user == null) return BadRequest(new {message = "User not found"});
        var jwt = _jwtUtils.GenerateToken(user);
        return Ok(new UserLoginResponseDto(user.UserId, user.Username, user.Email, jwt));
    }
    
    [Authorize]
    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        => Ok(await _userRepository.GetAllUsers());
}