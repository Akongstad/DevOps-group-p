namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger; 
    private readonly IAuthenticator _authenticator;
    private readonly IUserRepository _repository;
    private readonly IJwtUtils _jwtUtils;
    public AuthenticationController(ILogger<AuthenticationController> logger, IUserRepository repository, IAuthenticator authenticator, IJwtUtils jwtUtils)
    {
        _logger = logger;
        _repository = repository;
        _authenticator = authenticator;
        _jwtUtils = jwtUtils;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> GetLogin([FromBody] UserLoginDto login)
    {
        var user = await  _repository.GetUserByName(login.Username);
        if (user is null) return BadRequest(new {message = "Invalid Username"});
        
        var id = await _authenticator.Login(user.Username, login.PwHash);
        if(id < 1) return BadRequest(new {message = "Invalid Password"});

        var jwt = _jwtUtils.GenerateToken(user);
        
        return Ok(new UserLoginResponseDto(user.UserId, user.Username, user.Email, jwt));
    }
    
    [HttpPost("register")]                                                          
    public async Task<IActionResult> PostRegister ([FromBody] UserCreateDto user)
    {
        var id = await _authenticator.Register(user.Username, user.Email, user.PwHash);
        if (id < 1)
            return Conflict(new {message = "Invalid username"});
        return Created("success", await _repository.GetUserById(id));    
    }  
    
}