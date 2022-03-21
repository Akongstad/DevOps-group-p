namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger; //??
    private readonly IUserRepository _repository;

    public UserController(ILogger<UserController> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    // Get Public timeline'
    [HttpGet("Users")]
    public Task<IEnumerable<UserDto>> Get(){
        return _repository.GetAllUsers();
    }
    //[AutoValidateAntiForgeryToken]
    [HttpGet("messages")]
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
        var timeline = await _repository.GetPublicTimeline();
        return await SerializeTimeline(timeline);
    } 
   
    // Get User's timeline
    [HttpGet("messages/{username}")]
    public async Task<IActionResult> GetTimeline(string username)
    {
        var sessionId = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(sessionId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _repository.GetTimeline(sessionId, username);

        return Ok(timeline);
    }

    [HttpPost("message/{username}")]
    public async Task<IActionResult> Message([FromBody] MessageCreateDto message)
    {
        var id = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _repository.PostMessage(id, message.Text);
        return result.ToActionResult();
    }

    private async Task<bool> ValidateId(long id){
        return await _repository.GetUserDetailsById(id) == null;
    }

    private static Task<ActionResult<string>> SerializeTimeline (IEnumerable<MessageDto> timeline)
    {
        var messageList = timeline.Select(item => new {content = item.Text, pub_date = item.PubDate, user = item.Author,}).Cast<object>().ToList();
        return Task.FromResult<ActionResult<string>>(JsonSerializer.Serialize(messageList));
    }
}