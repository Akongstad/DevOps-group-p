namespace MinitwitReact.Core.DTO;

public record FollowerDto(long FollowerId, string FollowedUsername);

public record FollowerCreateDto
{
    public long? WhoId { get; set; }
    public long? WhomId { get; set; }
}

public record FollowerUpdateDto : FollowerCreateDto
{
    public long WhoId {get; set; }
}