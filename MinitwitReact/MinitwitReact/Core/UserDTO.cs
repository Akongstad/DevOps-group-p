using System.ComponentModel.DataAnnotations;

namespace MinitwitReact.Core;

public record UserDto(int UserId, string Username);

public record UserDetailsDto(int UserId, string Username, string Email, string PwHash);

public record UserCreateDto
{
    [StringLength(50)]
    public string Username { get; init; } = null!;
    [StringLength(50)]
    public string Email { get; set; } = null!;
    [StringLength(100)]
    public string PwHash { get; init; } = null!;
}

public record UserUpdateDto : UserCreateDto
{
    public int UserId { get; set; }
}
