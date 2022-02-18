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

    //[AutoValidateAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
        var timeline = await _minitwit.PublicTimeline();
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
}