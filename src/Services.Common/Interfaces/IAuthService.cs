namespace Services.Common.Interfaces;

public interface IAuthService
{
    Task<bool> AuthenticateUser(string username, string password);
    Task<bool> RegisterUser(string username, string password);
}