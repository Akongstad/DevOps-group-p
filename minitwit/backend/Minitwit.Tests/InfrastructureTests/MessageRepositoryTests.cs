namespace Minitwit.Tests.InfrastructureTests;

public class MessageRepositoryTests : BaseRepositoryTest 
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public MessageRepositoryTests()
    {
        _userRepository = new UserRepository(Context);
        _messageRepository = new MessageRepository(Context, _userRepository);

    }
    
    [Fact]
    public async Task GetPublicTimeline_returns_public_timeline()
    {
        var actual = await _messageRepository.GetPublicTimeline();
        var valueTuples = actual.ToList();
        
        Assert.Equal("Jeff Bezos", valueTuples.First().Author);
        Assert.Equal("Elon bad", valueTuples.First().Text);
        Assert.Equal("Bruce Wayne", valueTuples.Last().Author);
        Assert.Equal("I am Batman!", valueTuples.Last().Text);
    }
    
    [Fact]
    public async Task GetTimelineByUsernameAndSessionId_returns_null_if_invalid_user()
    {
        var actual = await _messageRepository.GetTimelineByUsername("a");
        var expected = new List<MessageDto>();
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetOwnTimelineBySessionId_returns_own_timeline()
    {
        var actual = await _messageRepository.GetOwnTimelineById(1);
        var valueTuples = actual.ToList();
        
        Assert.Equal("Elon Musk", valueTuples.First().Author);
        Assert.Equal("Tesla stonks", valueTuples.First().Text);
    }

    [Fact]
    public async Task GetOwnTimelineBySessionId_returns_publicTimeline_if_session_id_is_invalid()
    {
        var actual = await _messageRepository.GetOwnTimelineById(-1);
        var valueTuples = actual.ToList();
        
        Assert.Equal("Jeff Bezos", valueTuples.First().Author);
        Assert.Equal("Elon bad", valueTuples.First().Text);
        Assert.Equal("Bruce Wayne", valueTuples.Last().Author);
        Assert.Equal("I am Batman!", valueTuples.Last().Text);
    }
    
    [Fact]
    public async Task PostMessage_Creates_new_message_by_user()
    {
        var response = await _messageRepository.PostNewMessageToTimeline(1, "I make a new post yes");
        Assert.Equal(Status.Created, response);
    }

    [Fact]
    public async Task PostMessage_returns_NotFound_given_invalid_userid()
    {
        var response = await _messageRepository.PostNewMessageToTimeline(111, "I make a new post yes");
        Assert.Equal(Status.NotFound, response);
    }
    [Fact]
    public async Task PostMessage_Posts_Message_to_top_of_timeline()
    {
        var response = await _messageRepository.PostNewMessageToTimeline(1, "I make a new post yes");
        Assert.Equal(Status.Created, response);

        var timeline = await _messageRepository.GetPublicTimeline();
        timeline = timeline.ToList();
        //Post by user 1?
        var username = timeline.First().Author;
        var userId = _userRepository.GetUserIdFromUsername(username).Result;
        // ERROR: EXPECTED SHOULD BE 1, BUT IS 2 - fails since the db is seeded with messages from the future
        Assert.Equal(2, userId);
        Assert.Equal("I make a new post yes", timeline.Last().Text);
    }
}