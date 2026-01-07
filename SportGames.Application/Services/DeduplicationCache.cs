using SportGames.Core.Interfaces;
using System.Collections.Concurrent;

namespace SportGames.Application.Services;

//In production: use Redis with TTL = 3hr to cover 2hr time window
internal class DeduplicationCache : IDeduplicationCache
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    public Task<bool> TryAdd(string key, CancellationToken cancellationToken)
    {
        return Task.FromResult(_cache.TryAdd(key, key));
    }
}
