using Domain.GameAggregate;
using ChessAnalyzerApi.Services.Lichess;

namespace ChessAnalyzerApi;

public static class RegistratorEvaluationServices
{
    public static IServiceCollection AddEvaluationServices(this IServiceCollection services) => services
    .AddScoped<IPositionEvaluation, LichessService>();
    //builder.Services.AddScoped<IPositionEvaluation, Stockfish>() // добавить!!
}
