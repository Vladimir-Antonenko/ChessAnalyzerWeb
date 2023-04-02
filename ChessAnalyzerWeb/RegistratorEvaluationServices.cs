using Domain.GameAggregate;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.Stockfish.NET.Models;

namespace ChessAnalyzerApi;

public static class RegistratorEvaluationServices
{
    public static IServiceCollection AddEvaluationServices(this IServiceCollection services) => services
    .AddScoped<IPositionEvaluation, LichessService>()
    .AddSingleton<IPositionEvaluation, Stockfish>();
}
