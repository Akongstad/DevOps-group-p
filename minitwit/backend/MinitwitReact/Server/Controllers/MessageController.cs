namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MessageController> _logger;
    

    public MessageController(ILogger<MessageController> logger, IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }
    
    // Add message
    [HttpPost("msg")]
    public async Task<IActionResult> PostNewMessage([FromBody] MessageCreateDto message)
    {
        var id = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(id)){
            throw new ArgumentException("user not logged in");
        }

        var result = await _messageRepository.PostMessageToTimeline(id, message.Text);
        return result.ToActionResult();
    }
    
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
        var timeline = await _messageRepository.GetPublicTimeline();
        return await SerializeTimeline(timeline);
    } 
    
    [HttpGet("messages/{username}")]
    public async Task<IActionResult> GetTimeline(string username)
    {
        var sessionId = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (await ValidateId(sessionId)){
            throw new ArgumentException("user not logged in");
        }
        var timeline = await _messageRepository.GetTimelineByUsernameAndSessionId(sessionId, username);

        return Ok(timeline);
    }

    private async Task<bool> ValidateId(long id){
        return await _userRepository.GetUserDetailsById(id) == null;
    }

    private static Task<ActionResult<string>> SerializeTimeline (IEnumerable<MessageDto> timeline)
    {
        var msgs = timeline.Select(item => new {content = item.Text, pub_date = item.PubDate, user = item.Author,}).Cast<object>().ToList();
        return Task.FromResult<ActionResult<string>>(JsonSerializer.Serialize(msgs));
    }
}