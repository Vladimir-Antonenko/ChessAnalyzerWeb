using Domain.GameAggregate;
using Infrastructure.Repositories;

namespace ChessAnalyzerApi.Registrators;

public static class RepositoryRegistrator
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services) => services
        .AddScoped<IPlayerRepository, PlayerRepository>();
}