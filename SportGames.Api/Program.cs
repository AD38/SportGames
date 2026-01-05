using SportGames.Core.Interfaces;
using SportGames.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/games", async (
    SportType? sportType,
    string? competition,
    DateTime? from,
    DateTime? to,
    int pageNumber,
    int pageSize,
    IGameRepository repository,
    CancellationToken ct) =>
    {
        var query = new GameQuery(
            sportType,
            competition,
            from,
            to,
            pageNumber,
            pageSize);

        var games = await repository.Get(query, ct);

        return Results.Ok(games);
    });

app.Run();
