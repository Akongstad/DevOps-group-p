//reference: https://jasonwatmore.com/post/2022/01/07/net-6-user-registration-and-login-tutorial-with-example-api
namespace MinitwitReact.Authentication;

public class AppSettings
{
    public AppSettings(string secret)
    {
        Secret = secret;
    }

    public string Secret { get;}
}