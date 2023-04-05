using Domain.GameAggregate;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;
using ChessAnalyzerApi.Services.Stockfish.NET.Models;

namespace ChessAnalyzerApi.Registrators;

public static class RegistratorEvaluationServices
{
    public static IServiceCollection AddEvaluationServices(this IServiceCollection services) => services
    .AddScoped<IPositionEvaluation, LichessService>()
    .AddScoped<IPositionEvaluation, ChessDBService>()
    .AddSingleton<IPositionEvaluation, Stockfish>();
}