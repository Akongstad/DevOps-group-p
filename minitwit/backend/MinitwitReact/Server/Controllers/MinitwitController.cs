using MinitwitReact.Core.DTO;

namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitController : ControllerBase
{
    private readonly ILogger<MinitwitController> _logger;
    private readonly IMinitwit _miniTwit;
    public MinitwitController(ILogger<MinitwitController> logger, IMinitwit miniTwit)
    {
        _logger = logger;
        _miniTwit = miniTwit;
    }

    // Get Public timeline
    [HttpGet("Users")]
    public Task<IEnumerable<UserDto>> Get(){
        return _miniTwit.GetAllUsers();
    }
    //[AutoValidateAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
       var timeline = await _miniTwit.PublicTimeline();
       return await SerializeTimeline(timeline);
    } 
   
    // Get User's timeline
    [HttpGet("msgs1/{sessionId}")]
    public async Task<ActionResult<string>> GetTimeline(int sessionId)
    {
        if (await ValidateId(sessionId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _miniTwit.OwnTimeline(sessionId);

        return await SerializeTimeline(timeline);
    }

    // Get other users' timeline
    [HttpGet("msgs/{username}")]
    public async Task<ActionResult<string>> GetUserTimeline(string username)
    {
        var userId = await _miniTwit.GetUserId(username);
        if (await ValidateId(userId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _miniTwit.UserTimeline(userId, username);
        return await SerializeTimeline(timeline);
    }

    // Follow
    // follower.userid = user's own id, follower.username = the other user's name
    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromBody] FollowerDto follower)
    {

        if (await ValidateId(follower.FollowerId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _miniTwit.FollowUser(follower.FollowerId, follower.FollowedUsername);
        return result.ToActionResult();
    }
    //Unfollow
    [HttpPost("unfollow")]
    public async Task<IActionResult> UnFollow([FromBody] FollowerDto follower)
    {
        if (await ValidateId(follower.FollowerId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _miniTwit.UnfollowUser(follower.FollowerId, follower.FollowedUsername);
        return result.ToActionResult();
    }

    // Login
    [HttpGet("login")]
    public async Task<long>GetLogin([FromBody] UserDetailsDto user)
    {
        return await _miniTwit.Login(user.Username, user.PwHash);
    }
    
    // Add message
    [HttpPost("msg/{id}")]
    public async Task<IActionResult> Message(long id, [FromBody] MessageCreateDto message)
    {        
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _miniTwit.PostMessage(id, message.Text);
         return result.ToActionResult();
    }

    //Register
    [HttpPost("register")]
    public async Task<long> PostRegister ([FromBody] UserCreateDto user)
    {
        return await _miniTwit.Register(user.Username, user.Email, user.PwHash);
    }


    //validate user
    private async Task<bool> ValidateId(long id){
        return await _miniTwit.GetUserDetailsById(id) == null;
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