namespace MinitwitReact.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitSimulationController : ControllerBase
{
    private readonly ILogger<MinitwitSimulationController> _logger;
    private readonly IMinitwit _minitwit;

    public MinitwitSimulationController(ILogger<MinitwitSimulationController> logger, IMinitwit minitwit)
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
        var timeline =  await _minitwit.PublicTimeline();
        var filtered_msgs = new List<Object>();
        foreach (var item in timeline)
        {
            var filtered_msg = new
            {
                content = item.Item1.Text,
                pub_date = item.Item1.PubDate,
                user = item.Item1.Author,
            };
            filtered_msgs.Add(filtered_msg);
        }
        return JsonSerializer.Serialize(filtered_msgs);
    } 
   
    // Get User's timeline
    [HttpGet("{id}")]
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> GetTimeline(int id)
    {
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        return await _minitwit.OwnTimeline(id);
    }

    // Get other users' timeline
    [HttpGet("msgs/{username}")]
    [HttpPost("msgs/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetUserTimeline(string username)
    {
        var userId = await _minitwit.GetUserId(username);
        if (Request.Method == "GET")
        {
            if (userId <= 0)
            {
                return NotFound();
            }

            var timeline = await _minitwit.UserTimeline(0, username);
            var filtered_msgs = new List<Object>();
            foreach (var item in timeline)
            {
                var filtered_msg = new
                {
                    content = item.Item1.Text,
                    pub_date = item.Item1.PubDate,
                    user = item.Item1.Author,
                };
                filtered_msgs.Add(filtered_msg);
            }

            return JsonSerializer.Serialize(filtered_msgs);
        }

        if (Request.Method == "POST")
        {
            var sessionId = await _minitwit.GetUserId(username);
            var message = Request.Headers["content"].ToString();
            var result = await _minitwit.PostMessageEf(sessionId, message);
            return NoContent();
        }
        return NotFound();
    }
    [HttpPost("msgs/{username}/{message}")]
    public async Task<IActionResult> Message(string username, string message)
    {
        var sessionId = await _minitwit.GetUserId(username);
        var result = await _minitwit.PostMessageEf(sessionId, message);
        return result.ToActionResult();
    }

    // Follow
    [HttpPost("follow/{id}/{username}")]
    public async Task<IActionResult> Follow(int id, string username){
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUserEf(id, username);
        return result.ToActionResult();
    }
    //Unfollow
    [HttpPost("/unfollow/{id}/{username}")]
    public async Task<IActionResult>  Unfollow(int id, string username){
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUserEf(id, username);
        return result.ToActionResult();
    }

    // Login
    [HttpGet("login/{username}/{pw_hash}")]
    public async Task<long>GetLogin(string username, string pw_hash)
    {
        return await _minitwit.LoginEf(username, pw_hash);
    }

    //Register
    [HttpPost("{username}/{email}/{pw}")]
    public async Task<long> PostRegister (string username, string email, string pw)
    {
        return await _minitwit.RegisterEf(username, email, pw);
    }


    //validate user
    private async Task<bool> ValidateId(long id){
        return await _minitwit.GetUserDetialsById(id) == null;
    }
}