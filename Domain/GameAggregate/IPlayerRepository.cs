using Domain.Extensions;

namespace Domain.GameAggregate;

public interface IPlayerRepository : IRepositoryBase<Player>
{
    public Task<Player?> FindByName(string name);
    public Task<PagedList<Position>> GetMistakesWithPagination(string name, int pageNum, int pageSize);
}