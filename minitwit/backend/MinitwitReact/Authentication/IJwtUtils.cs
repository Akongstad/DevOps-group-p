//reference: https://jasonwatmore.com/post/2022/01/07/net-6-user-registration-and-login-tutorial-with-example-api
namespace MinitwitReact.Authentication;

public interface IJwtUtils
{
    public string GenerateToken(UserDetailsDto user);
    public int? ValidateToken(string token);
}