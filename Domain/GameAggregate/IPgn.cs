namespace Domain.GameAggregate;

public interface IPgn
{
    /// <summary>
    /// Платформа с которой будут получены игры
    /// </summary>
    public ChessPlatform? Platform { get; }

    public Task<Pgn> GetPgnGamesAsync(string login, DateTime? since, DateTime? until);
}
