using System.Text.Json.Nodes;

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
            var request = await Request.ReadFromJsonAsync<JsonObject>();
            var result = await _minitwit.PostMessage(sessionId, request!["content"]!.ToString());
            return NoContent();
        }
        return NotFound();
    }
    [HttpGet("fllws/{username}")]
    [HttpPost("fllws/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<string>> Follow(string username)
    {
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
            return NotFound();
        }
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        var limit = 100;
        request.TryGetPropertyValue("no", out var no);
        if (!int.TryParse(no.ToString(), out limit))
        {
            limit = 100;
        }

        if (Request.Method == "POST" & request!.ContainsKey("follow"))
        {
            var followUsername = request["follow"]!.ToString();
            var followsUserId = await _minitwit.GetUserId(followUsername);
            if(followsUserId <= 0)
            {
                return NotFound();
            }
            var result = await _minitwit.FollowUser(userId, followUsername);
            return NoContent();
        }
        if(Request.Method =="POST" && request.ContainsKey("unfollow"))
        {
            var unfollowUsername = request["unfollow"]!.ToString();
            var unfollowsUserId = await _minitwit.GetUserId(unfollowUsername);
            if (unfollowsUserId <= 0)
            {
                return NotFound();
            }
            var result = await _minitwit.UnfollowUser(userId, unfollowUsername);
            return NoContent();
        }
        if (Request.Method == "GET")
        {
            var followers = await _minitwit.GetFollowers(username, limit);
            var filtered_msgs = new List<Object>();
            foreach (var item in followers)
            {
                var filtered_msg = new
                {
                    follows = item.Username
                };
                filtered_msgs.Add(filtered_msg);
            }
            return JsonSerializer.Serialize(filtered_msgs);
        }
        return Conflict();
    }
    
    
}