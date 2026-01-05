using SportGames.Core.Models;

namespace SportGames.Core.Interfaces;

public interface IGameDataRetrievingService
{
    string SourceId { get; }
    Task<IReadOnlyCollection<Game>> GetGames(CancellationToken cancellationToken);
}
