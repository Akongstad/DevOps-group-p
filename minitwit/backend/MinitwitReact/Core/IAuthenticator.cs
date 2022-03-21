namespace MinitwitReact.Core.IRepositories;

public interface IAuthenticator
{
    Task<long> Login(string username, string pw);
    Task<long> Register(string username, string email, string pw);
}