namespace MinitwitReact.Core.IRepositories;

public interface IMessageRepository
{
    Task<Status> PostNewMessageToTimeline(long userId, string message);
    Task<IEnumerable<MessageDto>> GetPublicTimeline();
    Task<IEnumerable<MessageDto>> GetTimelineByUsername(string username);
    Task<IEnumerable<MessageDto>> GetOwnTimelineById(long userId);
}