using MinitwitReact.Authentication;

namespace MinitwitReact.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitController : ControllerBase
{
    private readonly ILogger<MinitwitController> _logger;
    private readonly IMinitwit _minitwit;
    private readonly IJwtUtils _jwtUtils;

    public MinitwitController(ILogger<MinitwitController> logger, IMinitwit minitwit, IJwtUtils jwtUtils)
    {
        _logger = logger;
        _minitwit = minitwit;
        _jwtUtils = jwtUtils;
    }

    // Get Public timeline'
    [HttpGet("Users")]
    public Task<IEnumerable<UserDto>> Get(){
        return _minitwit.GetAllUsers();
    }
    //[AutoValidateAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
       var timeline = await _minitwit.PublicTimeline();
       return await SerializeTimeline(timeline);
    } 
   
    // Get User's timeline
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetTimeline(string username)
    {
        var sessionId = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(sessionId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _minitwit.UserTimeline(sessionId, username);

        return Ok(timeline);
    }

    // Follow
    // follower.userid = user's own id, follower.username = the other user's name
    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromBody] FollowerDto follower)
    {

        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }
    //Unfollow
    [HttpPost("unfollow")]
    public async Task<IActionResult> UnFollow([FromBody] FollowerDto follower)
    {
        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.UnfollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }

    // Login
    [HttpPost("login")]
    public async Task<IActionResult> GetLogin([FromBody] UserLoginDto login)
    {
        var user = await  _minitwit.UserByName(login.Username);
        if (user is null) return BadRequest(new {message = "Invalid Username"});
        
        var id = await _minitwit.Login(user.Username, login.PwHash);
        if(id < 1) return BadRequest(new {message = "Invalid Password"});

        var jwt = _jwtUtils.GenerateToken(user);
        
        return Ok(new UserLoginResponseDto(user.UserId, user.Username, user.Email, jwt));
    }
    
    //Register                                                                      
    [HttpPost("register")]                                                          
    public async Task<IActionResult> PostRegister ([FromBody] UserCreateDto user)
    {
        var id = await _minitwit.Register(user.Username, user.Email, user.PwHash);
        if (id < 1)
            return Conflict(new {message = "Invalid username"});
        return Created("success", await _minitwit.GetUserById(id));    
    }                                                                               
    
    // Add message
    [HttpPost("msg/{username}")]
    public async Task<IActionResult> Message([FromBody] MessageCreateDto message)
    {
        var id = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _minitwit.PostMessage(id, message.Text);
         return result.ToActionResult();
    }

    //validate user
    private async Task<bool> ValidateId(long id){
        return await _minitwit.GetUserDetailsById(id) == null;
    }

    private static Task<ActionResult<string>> SerializeTimeline (IEnumerable<MessageDto> timeline)
    {
        var msgs = new List<object>();
        foreach (var item in timeline)
        {
            var msg = new
            {
                content = item.Text,
                pub_date = item.PubDate,
                user = item.Author,
            };
            msgs.Add(msg);
        }
        return Task.FromResult<ActionResult<string>>(JsonSerializer.Serialize(msgs));
      
    }
}