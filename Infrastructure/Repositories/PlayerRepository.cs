using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PlayerRepository : RepositoryBase<Player>
{
    public PlayerRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Player?> FindByName(string name)
        => await FindByCondition(x => x.Name == name, trackChanges: true).FirstOrDefaultAsync();
}