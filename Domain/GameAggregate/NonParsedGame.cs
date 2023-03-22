namespace Domain.GameAggregate;

public class NonParsedGame
{
    public int Id { get; private set; }
    public Game Game { get; private set; }
    public int GameId { get; private set; }
    private NonParsedGame(Game game)
    {
        Game = game;
    }

    public static NonParsedGame Create(Game game) => new(game);
}
