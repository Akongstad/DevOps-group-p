//Reference: https://jasonwatmore.com/post/2022/01/07/net-6-user-registration-and-login-tutorial-with-example-api

namespace MinitwitReact.Authentication;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IMinitwit minitwit, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateToken(token ?? string.Empty);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = minitwit.GetUserById(userId.Value);
        }

        await _next(context);
    }
}