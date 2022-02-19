namespace MinitwitReact.Core;

public record FollowerDTO(int UserId, string Username);

public record FollowerDetailsDto(int UserId, string Username, string Email, string PwHash);

public record FollowerCreateDto
{
    public long? WhoId { get; set; }
    public long? WhomId { get; set; }
}

public record FollowerUpdateDto : FollowerCreateDto
{
    public long WhoId {get; set; }
}