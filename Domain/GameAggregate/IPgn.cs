namespace Domain.GameAggregate;

public interface IPgn
{
    public Task<Pgn> GetPgnGamesAsync(string login, DateTime since = default, DateTime until = default);
}
