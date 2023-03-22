namespace Domain.GameAggregate;

public interface IPgn
{
    public Task<Pgn> GetAllPgnGamesAsync();
}
