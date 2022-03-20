namespace MinitwitReact.Core;

public record FollowerDto(int UserId, string Username);

public record FollowerCreateDto
{
    public long? WhoId { get; set; }
    public long? WhomId { get; set; }
}

public record FollowerUpdateDto : FollowerCreateDto
{
    public long WhoId {get; set; }
}