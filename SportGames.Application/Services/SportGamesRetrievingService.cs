using SportGames.Core.Interfaces;
using SportGames.Core.Models;

namespace SportGames.Application.Services;

//In production use HttpClient to get data and parse HTML if source doesn't have API,
//probably some normalization for date and team names needed
internal class SportGamesRetrievingService : IGameDataRetrievingService
{
    public string SourceId => "MockedSource";

    public Task<IReadOnlyCollection<Game>> GetGames(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Game> mockedGames =
        [
            new Game(
                Guid.NewGuid(),
                DateTime.UtcNow.AddHours(-3),
                SportType.Football,
                "Premier League",
                new TeamPair("Brenfort", "Sunderland")),
            new Game(
                Guid.NewGuid(),
                DateTime.UtcNow.AddMinutes(-30),
                SportType.Football,
                "Super Cup",
                new TeamPair("Barcelona", "Athletic Club")),
        ];

        return Task.FromResult(mockedGames);
    }
}
