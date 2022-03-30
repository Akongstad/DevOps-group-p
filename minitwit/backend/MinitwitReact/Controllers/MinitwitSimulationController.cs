using Microsoft.AspNetCore.Http.Extensions;
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
    public  string GetLatest()
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
        var filteredMsgs = new List<object>();
        foreach (var item in timeline)
        {
            var filteredMsg = new
            {
                content = item.Text,
                pub_date = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(item.PubDate), TimeZoneInfo.Local).ToString("hh:mm tt ddd"),
                user = item.Author,
            };
            filteredMsgs.Add(filteredMsg); 
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
            var filteredMsgs = new List<object>();
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
            var request =await Request.ReadFromJsonAsync<JsonObject>();
            _tweetCounter.Inc();
            await _minitwit.PostMessage(userId, request!["content"]!.ToString());
            return NoContent();
        }
        return NotFound();
    }

    private readonly Counter _followCounter = 
        Metrics.CreateCounter("minitwit_follow_count", "Amount of calls to follow");
    private readonly Counter _unfollowCounter = 
        Metrics.CreateCounter("minitwit_unfollow_count", "Amount of calls to unfollow");
    private readonly Counter _followUserNofFoundCounter = 
        Metrics.CreateCounter("minitwit_follow_user_not_found_count", "Amount of calls to to follow/unfollow resulting in user not found");
    
    [HttpGet("fllws/{username}")]
    [HttpPost("fllws/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(string username)
    {
        _logger.LogDebug("Uri: {Uri}", Request.GetDisplayUrl());
        _logger.LogDebug("Follow request {Headers}\n" +
                         "{@Body}", Request.Headers, Request.Body );

        UpdateLatest(Request);
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
            _followUserNofFoundCounter.Inc();
            _userNotFoundCounter.Inc();
            return NotFound();
        }
        int limit = 100;
        if (Request.Method == "GET") 
        {
            return await GetFollows(username, limit);
        }

        var request = await Request.ReadFromJsonAsync<JsonObject>();
        if (Request.Method == "POST" & request.ContainsKey("follow"))
        {
            _followCounter.Inc();
            var followUsername = request["follow"]!.ToString();
            await _minitwit.FollowUser(userId, followUsername);
            return NoContent();
        }
        if(Request.Method =="POST" && request.ContainsKey("unfollow"))
        {
            _unfollowCounter.Inc();
            var unfollowUsername = request["unfollow"]!.ToString();
            await _minitwit.UnfollowUser(userId, unfollowUsername);
            return NoContent();
        }
        return BadRequest();

        
    }
    private async Task<IActionResult> GetFollows(string username, int limit)
    {
        var followers = await _minitwit.GetFollowers(username, limit);
        var filteredMsgs = new List<object>();
        foreach (var item in followers)
        {
            var filteredMsg = new
            {
                follows = item.Username
            };
            filteredMsgs.Add(filteredMsg);
        }
        return Ok(filteredMsgs);
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

        var id = await _minitwit.GetUserId(request!["username"]!.ToString());
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
            await _minitwit.Register(request["username"]!.ToString(),
                request["email"]!.ToString(), request["pwd"]!.ToString());
        }

        if (error != "")
        {
            _registerErrorCounter.Inc();
            return BadRequest(error);
        }
        return NoContent();

    }
}
