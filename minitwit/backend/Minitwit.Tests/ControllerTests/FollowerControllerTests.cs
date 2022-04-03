namespace Minitwit.Tests.ControllerTests;

public class FollowerControllerTests
{
    [Fact]
    public async Task PostNewMessageToTimeline_creates_new_Message()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1,"Elon Musk");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.FollowUser(follower)).ReturnsAsync(Status.Updated);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.Follow(follower);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async Task Follow_given_invalid_FollowerId_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(-1 ,"");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.FollowUser(follower )).ReturnsAsync(Status.NotFound);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.Follow(follower);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public async Task Follow_given_invalid_Followed_Username_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1 ,"");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.FollowUser(follower )).ReturnsAsync(Status.NotFound);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.Follow(follower);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public async Task Follow_given_same_FollowerId_and_Followed_Username_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1 ,"Elon Musk");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.FollowUser(follower)).ReturnsAsync(Status.Conflict);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.Follow(follower);

        // Assert
        Assert.IsType<ConflictResult>(response);
    }
    
    [Fact]
    public async Task UnFollow_updates_Users_followers_by_Deleting_Relation()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1,"Elon Musk");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.UnfollowUser(follower)).ReturnsAsync(Status.Updated);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.UnFollow(follower);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async Task UnFollow_given_invalid_FollowerId_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(-1 ,"");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.UnfollowUser(follower )).ReturnsAsync(Status.NotFound);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.UnFollow(follower);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public async Task UnFollow_given_invalid_Followed_Username_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1 ,"");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.UnfollowUser(follower)).ReturnsAsync(Status.NotFound);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.UnFollow(follower);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public async Task UnFollow_given_same_FollowerId_and_Followed_Username_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<FollowerController>>();
        var follower = new FollowerDto(1 ,"Elon Musk");
        var followerRepository = new Mock<IFollowerRepository>();
        followerRepository.Setup(m => m.UnfollowUser(follower)).ReturnsAsync(Status.Conflict);
        var controller = new FollowerController(logger.Object, followerRepository.Object);

        // Act
        var response = await controller.UnFollow(follower);

        // Assert
        Assert.IsType<ConflictResult>(response);
    }
    
}