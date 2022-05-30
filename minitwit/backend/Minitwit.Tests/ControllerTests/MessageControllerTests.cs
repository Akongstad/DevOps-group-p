/*namespace Minitwit.Tests.ControllerTests;

public class MessageControllerTests
{
    /*[Fact]
    public async Task PostNewMessageToTimeline_creates_new_Message()
    {
        // Arrange
        var logger = new Mock<ILogger<MessageController>>();
        var message = new MessageCreateDto();
        var messageRepository = new Mock<IMessageRepository>();
        messageRepository.Setup(m => m.PostNewMessageToTimeline(1, "!")).ReturnsAsync(Status.Created);
        var controller = new MessageController(logger.Object, messageRepository.Object);

        // Act
        var response = await controller.PostNewMessage(message);

        // Assert
        Assert.IsType<CreatedResult>(response);
    }

    [Fact]
    public async Task GetPublicTimeline_returns_IEnumerable_MessageDto()
    {
        // Arrange
        var logger = new Mock<ILogger<MessageController>>();
        var message = new List<MessageDto>();
        var messageRepository = new Mock<IMessageRepository>();
        messageRepository.Setup(m => m.GetPublicTimeline()).ReturnsAsync(message);
        var controller = new MessageController(logger.Object, messageRepository.Object);

        // Act
        var response = await controller.GetPublicTimeline();

        // Assert
        Assert.Equal(message, response);
    }#1#
}*/