namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger; //??
    private readonly IAuthenticator _authenticator;
    private readonly IUserRepository _userRepository;
    private readonly IJwtUtils _jwtUtils;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository, IAuthenticator authenticator, IJwtUtils jwtUtils)
    {
        _logger = logger;
        _userRepository = userRepository;
        _authenticator = authenticator;
        _jwtUtils = jwtUtils;
    }
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Register()
    {
        var request = await Request.ReadFromJsonAsync<JsonObject>();

        var id = await _userRepository.GetUserIdFromUsername(request!["username"]!.ToString());
        var error = "";
        
        if (!request.ContainsKey("username"))
        {
            error = "you have to enter a username";
        }
        else if(!request.ContainsKey("email") || !request["email"]!.ToString().Contains('@'))
        {
            error = "You have to enter a valid email address";
        }
        else if (!request.ContainsKey("pwd"))
        {
            error = "You have to enter a password";
        }
        else if (id > 0)
        {
            error = "The username ios already taken";
        }
        else
        {
            await _authenticator.Register(request["username"]!.ToString(),
                request["email"]!.ToString(), request["pwd"]!.ToString());
        }

        if (error == "") return NoContent();
        return BadRequest(error);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto login)
    {
        var user = await  _userRepository.GetUserDetailsByName(login.Username);
        if (user is null) return BadRequest(new {message = "Invalid Username"});
        
        var id = await _authenticator.Login(user.Username, login.PwHash);
        if(id < 1) return BadRequest(new {message = "Invalid Password"});

        var jwt = _jwtUtils.GenerateToken(user);
        
        return Ok(new UserLoginResponseDto(user.UserId, user.Username, user.Email, jwt));
    }
    
    


}