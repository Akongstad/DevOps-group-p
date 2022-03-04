using System.Text.Json;

namespace MinitwitReact.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitController : ControllerBase
{
    private readonly ILogger<MinitwitController> _logger;
    private readonly IMinitwit _minitwit;

    public MinitwitController(ILogger<MinitwitController> logger, IMinitwit minitwit)
    {
        _logger = logger;
        _minitwit = minitwit;
    }

    // Get Public timeline
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
    [HttpGet("msgs1/{sessionId}")]
    public async Task<ActionResult<string>> GetTimeline(int sessionId)
    {
        if (await ValidateId(sessionId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _minitwit.OwnTimeline(sessionId);

        return await SerializeTimeline(timeline);
    }

    // Get other users' timeline
    [HttpGet("msgs/{username}")]
    public async Task<ActionResult<string>> GetUserTimeline(string username)
    {
        var userId = await _minitwit.GetUserId(username);
        if (await ValidateId(userId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _minitwit.UserTimeline(userId, username);
        return await SerializeTimeline(timeline);
    }

    // Follow
    // follower.userid = user's own id, follower.username = the other user's name
    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromBody] FollowerDTO follower)
    {

        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }
    //Unfollow
    [HttpPost("unfollow")]
    public async Task<IActionResult> UnFollow([FromBody] FollowerDTO follower)
    {
        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.UnfollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }

    // Login
    [HttpGet("login")]
    public async Task<long>GetLogin([FromBody] UserDetailsDto user)
    {
        return await _minitwit.Login(user.Username, user.PwHash);
    }
    
    // Add message
    [HttpPost("msg/{id}")]
    public async Task<IActionResult> Message(long id, [FromBody] MessageCreateDto message)
    {        
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _minitwit.PostMessage(id, message.Text);
         return result.ToActionResult();
    }

    //Register
    [HttpPost("register")]
    public async Task<long> PostRegister ([FromBody] UserCreateDto user)
    {
        return await _minitwit.Register(user.Username, user.Email, user.PwHash);
    }


    //validate user
    private async Task<bool> ValidateId(long id){
        return await _minitwit.GetUserDetailsById(id) == null;
    }

    private async Task<ActionResult<string>> SerializeTimeline (IEnumerable<MessageDto> timeline)
    {
        var msgs = new List<Object>();
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
        return JsonSerializer.Serialize(msgs);
      
    }
}