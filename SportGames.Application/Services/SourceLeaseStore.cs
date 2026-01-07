using SportGames.Core.Interfaces;
using System.Collections.Concurrent;

namespace SportGames.Application.Services;

//In production: use Redis + redlock for source distributed locking 
internal class SourceLeaseStore : ISourceLeaseStore
{
    private readonly ConcurrentDictionary<string, (string workerId, DateTimeOffset expiresIn)> _leaseStore = new();

    public Task ReleaseAsync(string sourceId, string workerId, CancellationToken cancellationToken)
    {
        if (_leaseStore.TryGetValue(sourceId, out var lease) && lease.workerId == workerId)
        {
            _leaseStore.TryRemove(sourceId, out _);
        }

        return Task.CompletedTask;
    }

    public Task<bool> RenewAsync(string sourceId, string workerId, TimeSpan ttl, CancellationToken cancellationToken)
    {
        if (_leaseStore.TryGetValue(sourceId, out var lease) && lease.workerId == workerId)
        {
            var now = DateTimeOffset.UtcNow;
            var expiresIn = now + ttl;

            lease.expiresIn = expiresIn;
        }

        return Task.FromResult(false);
    }

    public Task<bool> TryAcquireAsync(string sourceId, string workerId, TimeSpan ttl, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresIn = now + ttl;

        if (_leaseStore.TryGetValue(sourceId, out var lease) && lease.expiresIn <= now)
        {
            _leaseStore.TryRemove(sourceId, out _);
        }

        return Task.FromResult(_leaseStore.TryAdd(sourceId, (workerId, expiresIn)));
    }
}
