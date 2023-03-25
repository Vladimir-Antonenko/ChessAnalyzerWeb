namespace Domain.GameAggregate;

public interface IPlayerRepository : IRepositoryBase<Player>
{
    public Task<Player?> FindByName(string name);
}