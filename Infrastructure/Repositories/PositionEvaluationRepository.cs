using Domain.GameAggregate;

namespace Infrastructure.Repositories;

public class PositionEvaluationRepository : RepositoryBase<PositionEvaluation>
{
    public PositionEvaluationRepository(BaseContext context) : base(context)
    {
    }
}
