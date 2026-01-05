namespace SportGames.Core.Models;

public sealed record GameQuery(
    SportType? SportType,
    string? Competition,
    DateTime? From,
    DateTime? To,
    int PageNumber,
    int PageSize);
