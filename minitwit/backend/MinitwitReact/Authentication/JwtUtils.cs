

namespace MinitwitReact.Authentication;


public class JwtUtils : IJwtUtils
{
    private readonly AppSettings _appSettings;

    public JwtUtils(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public string GenerateToken(UserDetailsDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username), new(ClaimTypes.NameIdentifier, user.UserId.ToString())
        };
        // generate token that is valid for 1days
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret ?? "ed24e03d-20c7-464e-a5ea-c82f75c491965"));


        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}