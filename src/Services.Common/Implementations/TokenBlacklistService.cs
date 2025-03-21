using System.Collections.Concurrent;
using Services.Common.Interfaces;

namespace Services.Common.Implementations;

public class TokenBlacklistService: ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _invalidTokens = new();

    public void AddToken(string token, DateTime expiryDate)
    {
        _invalidTokens.TryAdd(token, expiryDate);
    }

    public bool IsTokenValid(string token)
    {
        return !_invalidTokens.ContainsKey(token);
    }

    public void CleanupExpiredTokens()
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _invalidTokens)
        {
            if (kvp.Value < now)
            {
                _invalidTokens.TryRemove(kvp.Key, out _);
            }
        }
    }
}