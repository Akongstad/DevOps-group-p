namespace MinitwitReact.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger; //??
    private readonly IUserRepository _repository;

    public UserController(ILogger<UserController> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("Users")]
    public Task<IEnumerable<UserDto>> Get(){
        return _repository.GetAllUsers();
    }

    private async Task<bool> ValidateId(long id){
        return await _repository.GetUserDetailsById(id) == null;
    }

    private static Task<ActionResult<string>> SerializeTimeline (IEnumerable<MessageDto> timeline)
    {
        var messageList = timeline.Select(item => new {content = item.Text, pub_date = item.PubDate, user = item.Author,}).Cast<object>().ToList();
        return Task.FromResult<ActionResult<string>>(JsonSerializer.Serialize(messageList));
    }
}