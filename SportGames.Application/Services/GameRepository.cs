using SportGames.Core.Interfaces;
using SportGames.Core.Models;
using System.Collections.Concurrent;

namespace SportGames.Application.Services;

//In production: could be replaced with PostgreSQL, MS SQL; sharding is an option
//for heavy writing scenario Scylla could be used
//for game analytics ClickHouse could be used
internal class GameRepository : IGameRepository
{
    private ConcurrentBag<Game> _games = new();

    public Task<IEnumerable<Game>> Get(GameQuery query, CancellationToken cancellationToken)
    {
        var result = _games.AsEnumerable();

        if (query.From.HasValue)
            result = result.Where(g => g.StartTime >= query.From.Value);
        if(query.To.HasValue)
            result = result.Where(g => g.StartTime <= query.To.Value);
        if (!string.IsNullOrWhiteSpace(query.Competition))
            result = result.Where(g => string.Equals(g.CompetitionName, query.Competition, StringComparison.OrdinalIgnoreCase));
        if(query.SportType.HasValue)
            result = result.Where(g => g.SportType == query.SportType.Value);

        result = result.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

        return Task.FromResult(result);
    }

    public Task<IEnumerable<Game>> GetAll(CancellationToken cancellationToken) => Task.FromResult(_games.AsEnumerable());

    public Task Save(IReadOnlyList<Game> games, CancellationToken cancellationToken)
    {
        foreach (var game in games)
        {
            _games.Add(game);
        }

        return Task.CompletedTask;
    }
}
