namespace Minitwit.Tests.InfrastructureTests;

public class FollowerRepositoryTests : BaseRepositoryTest
{
    private readonly IFollowerRepository _followerRepository;

    public FollowerRepositoryTests()
    {
        _followerRepository = new FollowerRepository(Context, new UserRepository(Context));
    }
    
    [Fact]
    public async Task FollowUser_returns_updated_given_valid_session()
    {
        var response = await _followerRepository.FollowUser(new FollowerDto(1, "Jeff Bezos"));
        Assert.Equal(Status.Updated, response);
    }

    [Fact]
    public async Task FollowUser_returns_NotFound_given_invalid_target()
    {
        var response = await _followerRepository.FollowUser(new FollowerDto(1, "Jeffers"));
        Assert.Equal(Status.NotFound, response);
    }
    
    [Fact]
    public async Task FollowUser_returns_NotFound_given_invalid_session()
    {
        var response = await _followerRepository.FollowUser(new FollowerDto(-1, "Jeffers"));
        Assert.Equal(Status.NotFound, response);
    }

    [Fact]
    public async Task FollowUser_returns_Conflict_given_existing_follow()
    {
        var response = await _followerRepository.FollowUser(new FollowerDto(2, "Elon Musk"));
        Assert.Equal(Status.Conflict, response);
    }

    [Fact]
    public async Task FollowUser_add_follow_to_database()
    {
        var unused = await _followerRepository.FollowUser(new FollowerDto(1, "Jeff Bezos"));
        var jeff = new UserDto( 2, "Jeff Bezos");
        var follows = await _followerRepository.IsFollowing(1, jeff);
        Assert.True(follows); 
    }

    [Fact]
    public async Task UnFollowUser_returns_updated_given_valid_session_and_target()
    {
        var response = await _followerRepository.UnfollowUser(new FollowerDto(2, "Elon Musk"));
        Assert.Equal(Status.Updated, response);
    }   

    [Fact]
    public async Task UnFollowUser_returns_Notfound_given_invalid_target()
    {
        var response = await _followerRepository.UnfollowUser(new FollowerDto(2, "Elonis"));
        Assert.Equal(Status.NotFound, response);
    }
    
    [Fact]
    public async Task UnFollowUser_returns_Notfound_given_invalid_session_minus_1()
    {
        var response = await _followerRepository.UnfollowUser(new FollowerDto( -1, "Elon Musk"));
        Assert.Equal(Status.NotFound, response);
    }
    
    [Fact]
    public async Task UnFollowUser_returns_Conflict_given_nonExisting_follow()
    {
        var response = await _followerRepository.UnfollowUser(new FollowerDto(1, "Jeff Bezos"));
        Assert.Equal(Status.Conflict, response);
    }
    [Fact]
    public async Task UnfollowUser_removes_follow_to_database()
    {
        var elon = new UserDto(1, "Elon Musk");
        var follows = await _followerRepository.IsFollowing(2, elon);
        Assert.True(follows);
        
        var unused = await _followerRepository.UnfollowUser(new FollowerDto(2, "Elon Musk"));
        var unfollows = await _followerRepository.IsFollowing(2, elon);
        Assert.False(unfollows);
    }
    
    [Fact]
    public async Task GetFollowers_returns_List_of_followers()
    {
        const string bezos = "Jeff Bezos";
        const string elon = "Elon Musk";
        var actual = await _followerRepository.GetFollowersByUsernameWithLimit(bezos, 5);
        Assert.Equal(elon,actual.First().Username);
    }
    
    [Fact]
    public async Task GetFollowers_returns_EmptyList_of_followers_if_no_follows()
    {
        const string elon = "Elon Musk";
        var actual = await _followerRepository.GetFollowersByUsernameWithLimit(elon, 5);
        Assert.Empty(actual);
    }
    
    [Fact]
    public async Task GetFollowers_returns_EmptyList_of_followers_if_not_user()
    {
        var actual = await _followerRepository.GetFollowersByUsernameWithLimit("else", 5);
        Assert.Empty(actual);
    }
}