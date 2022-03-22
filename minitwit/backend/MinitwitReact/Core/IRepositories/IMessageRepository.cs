namespace MinitwitReact.Core.IRepositories;

public interface IMessageRepository
{
    Task<Status> PostMessageToTimeline(long userid, string message);
    Task<IEnumerable<MessageDto>> GetPublicTimeline();
    Task<IEnumerable<MessageDto>> GetTimelineByUsernameAndSessionId(long sessionId, string username);
    Task<IEnumerable<MessageDto>> GetOwnTimelineBySessionId(long sessionId);
}