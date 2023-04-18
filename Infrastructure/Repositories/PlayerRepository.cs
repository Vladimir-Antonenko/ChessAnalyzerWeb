using Domain.Common;
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
        return await FindByCondition(x => x.Name == name, trackChanges: true) // вместо FindByCondition можно .Where("Name==@0", name) в System.Linq.Dynamic.Core
                    .Include(x => x.Games)
                    .ThenInclude(x => x.Positions)
                    .FirstOrDefaultAsync();
    }

    public async Task<Player?> FindByNameOnPlatform(string name, ChessPlatform platform)
    {
        return await FindByCondition(x => x.Name == name, trackChanges: true)
                    .Include(x => x.Games.Where(g => g.Platform.Equals(platform)))
                    .ThenInclude(x => x.Positions)
                    .FirstOrDefaultAsync();
    }

    public async Task<PagedList<Position>> GetMistakesWithPagination(string name, ChessPlatform platform, int pageNum, int pageSize)
    {
        return await FindByCondition(x => x.Name == name, trackChanges: false)
                    .Include(x => x.Games.Where(g => g.Platform.Equals(platform)))
                    .ThenInclude(x => x.Positions)
                    .SelectMany(x => x.Games)
                    .SelectMany(x => x.Positions.Where(x => x.IsMistake)) // фильтрую в SelectMany (в ThenInclude нельзя)
                    .ToPagedListAsync(pageNum, pageSize, indexFrom: 1);
    }   
}