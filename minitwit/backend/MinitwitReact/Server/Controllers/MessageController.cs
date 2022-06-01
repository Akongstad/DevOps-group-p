using MinitwitReact.Server.Extensions;

namespace MinitwitReact.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")] 
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;

    public MessageController(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    [AllowAnonymous]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PostNewMessage([FromBody] MessageCreateDto message)
        => (await _messageRepository.PostNewMessageToTimeline(Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier)), message.Text)).ToActionResult();


    [AllowAnonymous]
    [HttpGet("timeline")]
    [ProducesResponseType(typeof(IEnumerable<MessageToFrontendDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPublicTimeline()
    {
        var messages = await _messageRepository.GetPublicTimeline();
        var timelineMsgs = messages.Select(item =>
            new MessageToFrontendDto(MessageId: item.MessageId,
                Author: item.Author,
                Text: item.Text,
                PubDate: new DateTime(item.PubDate).AddHours(2).ToString("hh:mm tt ddd").ToString())).ToList();
        return Ok(timelineMsgs);
    }
    
    [AllowAnonymous]
    [HttpGet("timeline/{username}")]
    [ProducesResponseType(typeof(IEnumerable<MessageToFrontendDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTimeline(string username)
    {
        var messages = await _messageRepository.GetTimelineByUsername(username);
        var userTimelineMsgs = messages.Select(item =>
            new MessageToFrontendDto(MessageId: item.MessageId,
                Author: item.Author,
                Text: item.Text,
                PubDate: new DateTime(item.PubDate).AddHours(2).ToString("hh:mm tt ddd").ToString())).ToList();
        return Ok(userTimelineMsgs);
    }

}