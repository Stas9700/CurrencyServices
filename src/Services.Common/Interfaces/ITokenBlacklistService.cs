namespace Services.Common.Interfaces;

public interface ITokenBlacklistService
{
    void AddToken(string token, DateTime expiryDate);
    bool IsTokenValid(string token);
    void CleanupExpiredTokens();
}