using Domain.GameAggregate;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class RepositoryRegistrator
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services) => services
        .AddScoped<IPlayerRepository, PlayerRepository>();
}