namespace SportGames.Core.Models;

public record Game(Guid Id, DateTime StartTime, SportType SportType, string CompetitionName, TeamPair TeamPair)
{
    public string GetNormalizedKey()
    {
        var normalizedTeams = TeamPair.GetNormalizedKey();
        var timestamp = new DateTimeOffset(StartTime).ToUnixTimeMilliseconds();
        var windowTime = timestamp / (2 * 60 * 60 * 1000);

        return $"{SportType}:{CompetitionName}:{normalizedTeams}:{windowTime}";
    }
}
