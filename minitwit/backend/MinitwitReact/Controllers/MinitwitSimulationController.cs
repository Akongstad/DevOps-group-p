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

    private void UpdateLatest(JsonObject? request)
    {
        if (request == null)
        {
            _latest = -1;
            return;
        }
        var no = request["latest"];
        if (no ==null)
        {
            _latest = -1;
            return;
                
        }
        _latest = Int32.Parse(no.ToString());
    }

    [HttpGet("latest")]
    public async Task<string> GetLatest()
    {
        return JsonSerializer.Serialize(new {latest = _latest});
    }

    //[AutoValidateAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<ActionResult<string>> GetPublicTimeline()
    {
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(request);
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
        var request =await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(request);
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
            return NotFound();
        }
        if (Request.Method == "GET")
        {
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
            var result = await _minitwit.PostMessage(sessionId, request!["content"]!.ToString());
            return NoContent();
        }
        return NotFound();
    }
    [HttpGet("fllws/{username}")]
    [HttpPost("fllws/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<string>> Follow(string username)
    {
        var request =await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(request);
        var userId = await _minitwit.GetUserId(username);
        if (userId <= 0)
        {
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

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Register()
    {
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        UpdateLatest(request);
        
        
        var id = await _minitwit.GetUserId(request["username"].ToString());
        var error = "";
        
        if (!request.ContainsKey("username"))
        {
            error = "you have to enter a username";
        }
        else if(!request.ContainsKey("email") || !request["email"].ToString().Contains("@"))
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
            _minitwit.Register(request["username"].ToString(),
                request["email"].ToString(), request["pwd"].ToString());
        }

        if (error != "")
        {

            return JsonSerializer.Serialize(new {status = 400, error_msg = error});
        }
        return NoContent();

    }
}