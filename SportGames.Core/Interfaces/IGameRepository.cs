using SportGames.Core.Models;

namespace SportGames.Core.Interfaces;

public interface IGameRepository
{
    Task Save(IReadOnlyList<Game> games, CancellationToken cancellationToken);
    Task<Game> GetAll(CancellationToken cancellationToken);
    Task<Game> Get(GameQuery query, CancellationToken cancellationToken);
}
