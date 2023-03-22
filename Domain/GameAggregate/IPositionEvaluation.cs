namespace Domain.GameAggregate;

public interface IPositionEvaluation
{
    public Task<PositionEvaluation> GetPositionEvaluationAsync(string fen);
}
