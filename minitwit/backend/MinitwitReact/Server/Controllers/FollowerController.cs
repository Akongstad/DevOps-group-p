namespace MinitwitReact.Server.Controllers;

[Route("[controller]")]
public class FollowerController : ControllerBase
{
    private readonly IFollowerRepository _followerRepository;
    private readonly IUserRepository _userRepository;
    public FollowerController(IFollowerRepository followerRepository, IUserRepository userRepository)
    {
        _followerRepository = followerRepository;
        _userRepository = userRepository;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Follow([FromBody] FollowerDto follower)
    {

        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _followerRepository.FollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }
    
    [HttpPost("remove")]
    public async Task<IActionResult> UnFollow([FromBody] FollowerDto follower)
    {
        if (await ValidateId(follower.UserId)){
            throw new ArgumentException("user not logged in");
        }
        var result = await _followerRepository.UnfollowUser(follower.UserId, follower.Username);
        return result.ToActionResult();
    }
    
    private async Task<bool> ValidateId(long id){
        return await _userRepository.GetUserDetailsById(id) == null;
    }
}