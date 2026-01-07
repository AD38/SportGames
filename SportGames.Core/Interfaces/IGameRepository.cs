using SportGames.Core.Models;

namespace SportGames.Core.Interfaces;

public interface IGameRepository
{
    Task Save(IReadOnlyList<Game> games, CancellationToken cancellationToken);
    Task<IEnumerable<Game>> GetAll(CancellationToken cancellationToken);
    Task<IEnumerable<Game>> Get(GameQuery query, CancellationToken cancellationToken);
}
