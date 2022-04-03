namespace MinitwitReact.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FollowerController : ControllerBase
{
    private readonly ILogger<FollowerController> _logger; 
    private readonly IFollowerRepository _followerRepository;
    public FollowerController(ILogger<FollowerController> logger, IFollowerRepository followerRepository)
    {
        _logger = logger;
        _followerRepository = followerRepository;
    }

    [Authorize]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Follow([FromBody] FollowerDto follower)
        => (await _followerRepository.FollowUser(follower)).ToActionResult();

    [Authorize]
    [HttpPost("remove")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnFollow([FromBody] FollowerDto follower)
        => (await _followerRepository.UnfollowUser(follower)).ToActionResult();
}