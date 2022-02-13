using Microsoft.AspNetCore.Mvc;
using MinitwitReact.Entities;

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
    public IEnumerable<string> Get(){
        return _minitwit.GetUsers();
    }
    
    //[AutoValidateAntiforgeryToken]
    [HttpGet]
    public IEnumerable<(Message, User)> GetPublicTimeline()
    {
        return _minitwit.public_timeline();
    }
    // Get User's timeline
    [HttpGet("{id}")]
    public IEnumerable<(Message, User)> GetTimeline(int id)
    {
        if (validateID(id)){
            throw new ArgumentException("user not logged in");
        }
        return _minitwit.Timeline(id);
    }

    // Get other users' timeline
    [HttpGet("{id}/{username}")]
    public IEnumerable<(Message, User)> GetUserTimeline(int id,string username)
    {
        if (validateID(id)){
            throw new ArgumentException("user not logged in");
        }
        return _minitwit.user_timeline(id, username);
    }

    // Follow
    [HttpPost("follow/{id}/{username}")]
    public long Follow(int id, string username){
        if (validateID(id)){
            throw new ArgumentException("user not logged in");
        }
        return _minitwit.follow_user(username, id);
    }
    //Unfollow
    [HttpPost("/unfollow/{id}/{username}")]
    public long Unfollow(int id, string username){
        if (validateID(id)){
            throw new ArgumentException("user not logged in");
        }
        return _minitwit.unfollow_user(username, id);
    }

    // Login
    [HttpGet("login/{username}/{pw_hash}")]
    public long GetLogin(string username, string pw_hash)
    {
        return _minitwit.Login(username, pw_hash);
    }
    
    // Add message
    [HttpPost("{id}/{message}")]
    public string Message(long id, string message)
    {        
        if (validateID(id)){
            throw new ArgumentException("user not logged in");
        }
         return _minitwit.add_message(id, message);
    }

    //Register
    [HttpPost("{username}/{email}/{pw}")]
    public long PostRegister (string username, string email, string pw)
    {
        return _minitwit.Register(username, email, pw);
    }


    //validate user
    private bool validateID(long id){
        return (_minitwit.GetUser(id) == null);
    }
}
