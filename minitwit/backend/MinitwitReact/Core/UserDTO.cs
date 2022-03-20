using System.ComponentModel.DataAnnotations;

namespace MinitwitReact.Core;

// string Email, should maybe not be in the normal Dto like below
public record UserDto(long UserId, string Username);

public record UserLoginDto(string Username, string PwHash);
public record UserLoginResponseDto(long UserId, string Username, string Email, string Token);
public record UserDetailsDto(long UserId, string Username, string Email, string PwHash) : UserDto(UserId, Username);

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
