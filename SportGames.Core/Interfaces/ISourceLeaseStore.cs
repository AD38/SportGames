namespace SportGames.Core.Interfaces;

public interface ISourceLeaseStore
{
    Task<bool> TryAcquireAsync(string sourceId, string workerId, TimeSpan ttl, CancellationToken cancellationToken);
    Task<bool> RenewAsync(string sourceId, string workerId, TimeSpan ttl, CancellationToken cancellationToken);
    Task ReleaseAsync(string sourceId, string workerId, CancellationToken cancellationToken);
}
