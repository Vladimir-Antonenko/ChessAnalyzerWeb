using Domain.GameAggregate;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;
using ChessAnalyzerApi.Services.Stockfish.NET.Models;

namespace ChessAnalyzerApi.Registrators;

public static class RegistratorEvaluationServices
{
    /// <summary>
    /// Регистрирует сервисы оценки позиции
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEvaluationServices(this IServiceCollection services) 
        => services
                    .AddScoped<IPositionEvaluation, LichessService>()
                    .AddScoped<IPositionEvaluation, ChessDBService>()
                    .AddSingleton<IPositionEvaluation, Stockfish>();

    // на очереди ещё одни возможные api описанные тут https://snyk.io/advisor/npm-package/chess-web-api
}