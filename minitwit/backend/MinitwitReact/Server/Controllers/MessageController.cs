namespace MinitwitReact.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")] 
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger, IMessageRepository messageRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
    }
    
    [Authorize]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PostNewMessage([FromBody] MessageCreateDto message)
        => (await _messageRepository.PostNewMessageToTimeline(Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier)), message.Text)).ToActionResult();


    [AllowAnonymous]
    [HttpGet("timeline")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPublicTimeline()
        => Ok(await _messageRepository.GetPublicTimeline());


    [Authorize]
    [HttpGet("timeline/{username}")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTimeline(string username)
        => Ok(await _messageRepository.GetTimelineByUsername(username));
    
}