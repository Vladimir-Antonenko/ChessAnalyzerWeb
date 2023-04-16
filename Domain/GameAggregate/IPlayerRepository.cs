using Domain.Common;

namespace Domain.GameAggregate;

public interface IPlayerRepository : IRepositoryBase<Player>
{
    public Task<Player?> FindByName(string name);
    public Task<Player?> FindByNameOnPlatform(string name, ChessPlatform platform);
    public Task<PagedList<Position>> GetMistakesWithPagination(string name, ChessPlatform platform, int pageNum, int pageSize);
}