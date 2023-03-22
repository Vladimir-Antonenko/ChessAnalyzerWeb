using Domain.GameAggregate;

namespace Infrastructure.Repositories;

public class PositionRepository : RepositoryBase<Position>
{
    public PositionRepository(BaseContext context) : base(context)
    {
    }
}
