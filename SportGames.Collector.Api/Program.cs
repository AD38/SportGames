using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SportGames.Application;
using SportGames.Collector;
using SportGames.Core.Interfaces;
using SportGames.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/games", async (
    [FromServices] IGameRepository repository,
    CancellationToken cancellationToken) =>
{
    var games = await repository.GetAll(cancellationToken);

    return Results.Ok(games);
});

app.Run();

