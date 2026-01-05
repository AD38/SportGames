using System;
using System.Collections.Generic;
using System.Text;

namespace SportGames.Core.Models;

public record TeamPair(string TeamA, string TeamB)
{
    public string GetNormalizedKey() =>
        string.Compare(TeamA, TeamB, StringComparison.OrdinalIgnoreCase) < 0
            ? $"{TeamA}:{TeamB}"
            : $"{TeamB}:{TeamA}";
}
