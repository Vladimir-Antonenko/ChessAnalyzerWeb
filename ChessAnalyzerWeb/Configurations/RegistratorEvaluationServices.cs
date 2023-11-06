using Domain.GameAggregate;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;

namespace ChessAnalyzerApi.Configurations;

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
                    .AddSingleton<IPositionEvaluation, Services.Stockfish.NET.Models.Stockfish>();

    // на очереди ещё одни возможные api и БД с оценками (но без ходов)
}