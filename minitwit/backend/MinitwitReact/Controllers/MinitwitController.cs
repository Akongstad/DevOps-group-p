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
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> GetPublicTimeline()
    {
       return  await _minitwit.PublicTimeline();
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
    [HttpGet("{id}/{username}")]
    public async Task<IEnumerable<Tuple<MessageDto, UserDto>>> GetUserTimeline(int id,string username)
    {
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        return await _minitwit.UserTimeline(id, username);
    }

    // Follow
    [HttpPost("follow/{id}/{username}")]
    public async Task<IActionResult> Follow(int id, string username){
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUser(id, username);
        return result.ToActionResult();
    }
    //Unfollow
    [HttpPost("/unfollow/{id}/{username}")]
    public async Task<IActionResult>  Unfollow(int id, string username){
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _minitwit.FollowUser(id, username);
        return result.ToActionResult();
    }

    // Login
    [HttpGet("login/{username}/{pw_hash}")]
    public async Task<long>GetLogin(string username, string pw_hash)
    {
        return await _minitwit.Login(username, pw_hash);
    }
    
    // Add message
    [HttpPost("{id}/{message}")]
    public async Task<IActionResult> Message(long id, string message)
    {        
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _minitwit.PostMessage(id, message);
         return result.ToActionResult();
    }

    //Register
    [HttpPost("{username}/{email}/{pw}")]
    public async Task<long> PostRegister (string username, string email, string pw)
    {
        return await _minitwit.Register(username, email, pw);
    }


    //validate user
    private async Task<bool> ValidateId(long id){
        return await _minitwit.GetUserDetialsById(id) == null;
    }
}
