using Domain.Extensions;
using Domain.GameAggregate;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
{
    public PlayerRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Player?> FindByName(string name)
    {
        return await FindByCondition(x => x.Name == name, trackChanges: true)
                    .Include(x => x.Games)
                    .ThenInclude(x => x.Positions)
                    .Include(x => x.Mistakes)
                    .FirstOrDefaultAsync();
    }

    public async Task<PagedList<Position>> GetMistakesWithPagination(string name, int pageNum, int pageSize)
    {
        return await FindByCondition(x => x.Name == name, trackChanges: false)
                    .Include(x => x.Mistakes)
                    .SelectMany(x => x.Mistakes)
                    .ToPagedListAsync(pageNum, pageSize, indexFrom: 1);
    }    
}