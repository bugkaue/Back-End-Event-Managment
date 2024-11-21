using System.Collections.Concurrent;

namespace ProjectSolutisDevTrail.Services.Implementations;

public class TokenRevocationService
{
    private readonly ConcurrentDictionary<string, DateTime> revokedTokens = new();

    public void RevokeToken(string token)
    {
        revokedTokens[token] = DateTime.UtcNow;
    }

    public bool IsTokenRevoked(string token)
    {
        return revokedTokens.ContainsKey(token);
    }
}