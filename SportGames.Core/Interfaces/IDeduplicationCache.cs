namespace SportGames.Core.Interfaces
{
    public interface IDeduplicationCache
    {
        Task<bool> TryAdd(string key, CancellationToken cancellationToken);
    }
}
