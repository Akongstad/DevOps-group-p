namespace MinitwitReact.Server.Controllers;

[Route("follows/{username}")]
public class FollowerController : ControllerBase
{
    private readonly IFollowerRepository _followerRepository;
    private readonly IUserRepository _userRepository;
    public FollowerController(IFollowerRepository followerRepository, IUserRepository userRepository)
    {
        _followerRepository = followerRepository;
        _userRepository = userRepository;
    }
    
    [HttpGet("")]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(string username)
    {
        var request = await Request.ReadFromJsonAsync<JsonObject>();
        var userId = await _userRepository.GetUserIdFromUsername(username);
        if (userId <= 0)
        {
            return NotFound();
        }

        request!.TryGetPropertyValue("no", out var no);
        var limit = no == null ? 100 : int.Parse(no.ToString());

        if (Request.Method == "POST" & request.ContainsKey("follow"))
        {
            var followUsername = request["follow"]!.ToString();
            await _followerRepository.FollowUser(userId, followUsername);
            return NoContent();
        }
        if(Request.Method =="POST" && request.ContainsKey("unfollow"))
        {
            var unfollowUsername = request["unfollow"]!.ToString();
            await _followerRepository.UnfollowUser(userId, unfollowUsername);
            return NoContent();
        }

        if (Request.Method != "GET") return Conflict();
        {
            return await GetFollows(username, limit);
        }
    }
    private async Task<IActionResult> GetFollows(string username, int limit)
    {
        var followers = await _followerRepository.GetFollowers(username, limit);
        var filteredMessages = followers.Select(item => new {follows = item.Username}).Cast<object>().ToList();
        return Ok(filteredMessages);
    }
}