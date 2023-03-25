using Domain.GameAggregate;

namespace ChessAnalyzerApi.Services;
public interface IAnalyzeService
{
    public bool IsRunning();

    public bool HaveAnyEvaluationServises();

    public void AddAnalyzeService(IPositionEvaluation analysisService);

    public Task RunAnalyzePlayerGames(Player player, double mistakePrecision, CancellationToken token = default);
}