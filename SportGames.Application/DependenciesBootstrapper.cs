using Microsoft.Extensions.DependencyInjection;
using SportGames.Application.Services;
using SportGames.Core.Interfaces;

namespace SportGames.Application
{
    public static class DependenciesBootstrapper
    {
        public static IServiceCollection AddHostedApplication(this IServiceCollection services)
        {
            services.AddSingleton<IGameDataRetrievingService, SportGamesRetrievingService>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<IDeduplicationCache, DeduplicationCache>();
            services.AddSingleton<ISourceLeaseStore, SourceLeaseStore>();

            return services;
        }
    }
}
