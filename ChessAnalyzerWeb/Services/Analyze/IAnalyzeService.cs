using Domain.GameAggregate;

namespace ChessAnalyzerApi.Services.Analyze;
public interface IAnalyzeService
{
    public bool IsRunning();

    public bool HaveAnyEvaluationServises();

    public Task RunAnalyzePlayerGames(Player player, double mistakePrecision, CancellationToken token = default);
}