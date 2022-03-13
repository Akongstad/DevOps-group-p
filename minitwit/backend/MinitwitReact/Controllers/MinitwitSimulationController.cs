using Prometheus;

namespace MinitwitReact.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitSimulationController : ControllerBase
{
    private readonly ILogger<MinitwitSimulationController> _logger;
    private readonly IMinitwit _minitwit;
    private static int _latest;

    public MinitwitSimulationController(ILogger<MinitwitSimulationController> logger, IMinitwit minitwit)
    {
        _logger = logger;
        _minitwit = minitwit;
    }

    private readonly Counter _userNotFoundCounter =
        Metrics.CreateCounter("minitwit_user_not_found_count", "Calls resulting in user not found");
    private readonly Counter _updateLatestCounter =
        Metrics.CreateCounter("minitwit_update_latest_count", "Calls to update latest/requests served");
    private void UpdateLatest(HttpRequest request)
    {
        _updateLatestCounter.Inc();
        var no = request.Query["latest"];
        int.TryParse(no.ToString(), out var tmpLast);
        if (tmpLast > 0)
        {
            _latest = tmpLast;
        }
    }
    private readonly Counter _latestCounter =
        Metrics.CreateCounter("minitwit_latest_count", "Calls to latest");
    [HttpGet("latest")]
    public async Task<string> GetLatest()
    {
        _latestCounter.Inc();
        return JsonSerializer.Serialize(new {latest = _latest});
    }

    //[AutoValidateAntiforgeryToken]
    private readonly Counter _publicTimelineCounter = 
        Metrics.CreateCounter("minitwit_public_timeline_count", "Calls to public timeline get");
    [HttpGet("msgs")]
    public async Task<ActionResult> GetPublicTimeline()
    {
        _publicTimelineCounter.Inc();
        UpdateLatest(Request);
        var timeline = await _minitwit.PublicTimeline();
        var timeZone = TimeZoneInfo.Local;
        var filteredMsgs = new List<Object>();
        foreach (var item in timeline)
        {
            var filtered_msg = new
            {
                content = item.Text,
                pub_date = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(item.PubDate), TimeZoneInfo.Local).ToString("hh:mm tt ddd"),
                user = item.Author,
            };
            filteredMsgs.Add(filtered_msg);
        }
        return Ok(filteredMsgs);
    }

    private readonly Counter _tweetCounter = 
        Metrics.CreateCounter("minitwit_tweet_count", "Amount of calls to post message");
    private readonly Counter _tweetUserNotFoundCounter = 
        Metrics.CreateCounter("minitwit_tweet_user_not_found_count", "Amount of calls to post message resulting in user not found");
    // Get other users' timeline
    [HttpGet("msgs/{username}")]
    [HttpPost("msgs/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserTimeline([FromRoute]string username)
    {
        var request =await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(Request);
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
            _userNotFoundCounter.Inc();
            _tweetUserNotFoundCounter.Inc();
            return NotFound();
        }
        if (Request.Method == "GET")
        {
            var timeline = await _minitwit.UserTimeline(0, username);
            var filteredMsgs = new List<Object>();
            foreach (var item in timeline)
            {
                var filteredMsg = new
                {
                    content = item.Text,
                    pub_date = item.PubDate,
                    user = item.Author,
                };
                filteredMsgs.Add(filteredMsg);
            }
            return Ok(filteredMsgs);
        } 
        if (Request.Method == "POST")
        {
            _tweetCounter.Inc();
            var result = await _minitwit.PostMessage(userId, request!["content"]!.ToString());
            return NoContent();
        }
        return NotFound();
    }
    
    private readonly Counter _followCounter = 
        Metrics.CreateCounter("minitwit_follow_count", "Amount of calls to follow");
    private readonly Counter _unfollowCounter = 
        Metrics.CreateCounter("minitwit_unfollow_count", "Amount of calls to unfollow");
    [HttpGet("fllws/{username}")]
    [HttpPost("fllws/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(string username)
    {
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(Request);
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
            _userNotFoundCounter.Inc();
            return NotFound();
        }
        var limit = 100;
        request.TryGetPropertyValue("no", out var no);
        if (no == null)
        {
            limit = 100;
        }
        else
        {
            limit = int.Parse(no.ToString());
        }

        if (Request.Method == "POST" & request.ContainsKey("follow"))
        {
            _followCounter.Inc();
            var followUsername = request["follow"]!.ToString();
            var result = await _minitwit.FollowUser(userId, followUsername);
            return result.ToActionResult();
        }
        if(Request.Method =="POST" && request.ContainsKey("unfollow"))
        {
            _unfollowCounter.Inc();
            var unfollowUsername = request["unfollow"]!.ToString();
            var result = await _minitwit.UnfollowUser(userId, unfollowUsername);
            return result.ToActionResult();
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
            return Ok(filtered_msgs);
        }
        return Conflict();
    }
    
    
    private readonly Counter _registerCounter =
        Metrics.CreateCounter("minitwit_register_count", "Calls to register");
    private readonly Counter _registerErrorCounter =
        Metrics.CreateCounter("minitwit_register_error_count", "Calls to registern resulting in error");
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Register()
    {
        _registerCounter.Inc();
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(Request);
        
        
        var id = await _minitwit.GetUserId(request["username"].ToString());
        var error = "";
        
        if (!request.ContainsKey("username"))
        {
            error = "you have to enter a username";
        }
        else if(!request.ContainsKey("email") || !request["email"]!.ToString().Contains("@"))
        {
            error = "You have to enter a valid email address";
        }
        else if (!request.ContainsKey("pwd"))
        {
            error = "You have to enter a password";
        }
        else if (id > 0)
        {
            error = "The username ios already taken";
        }
        else
        {
            await _minitwit.Register(request["username"].ToString(),
                request["email"].ToString(), request["pwd"].ToString());
        }

        if (error != "")
        {
            _registerErrorCounter.Inc();
            return JsonSerializer.Serialize(new {status = 400, error_msg = error});
        }
        return NoContent();

    }
}
