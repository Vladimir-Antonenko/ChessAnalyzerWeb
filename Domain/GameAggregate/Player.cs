namespace Domain.GameAggregate;

public class Player
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Game> games = new();
    public IReadOnlyCollection<Game> Games => games;

    /// <summary>
    /// Для ef core
    /// </summary>
    private Player() { }

    /// <summary>
    /// Конструктор игрока
    /// </summary>
    /// <param name="userName">Логин игрока</param>
    private Player(string userName)
    {
        Name = userName;
    }

    /// <summary>
    /// Проверяет содержит ли игрок передаваемую игру
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public bool IsContainedThisGame(Game game) => games.Any(x => x.Pgn.Equals(game.Pgn));

    /// <summary>
    /// Определяет каким цветом игрок играл в данной игре
    /// </summary>
    /// <param name="game">Игра в которой нужно определить цвет игрока</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public Color GetPlayerColorInGame(Game game) =>
        game.WhiteGamer.Equals(this.Name) ?
            Color.White : game.BlackGamer.Equals(this.Name) ?
                Color.Black : throw new InvalidDataException($"Игра {game.Pgn}\n не принадлежит игроку");

    /// <summary>
    /// Добавляет игры игроку
    /// </summary>
    /// <param name="games">Добавляемые игры</param>
    public void AddGames(IEnumerable<Game> games)
    {
        foreach (var game in games)
            TryAddGame(game);
    }

    /// <summary>
    /// Добавляет игру в список игр игрока
    /// </summary>
    /// <param name="game">Добавляемая игра</param>
    /// <returns></returns>
    public bool TryAddGame(Game game)
    {
        if (!IsContainedThisGame(game) && game.HaveAnyPositions())
        {
            games.Add(game);
            return true;
        }    
        
        return false;
    }

    /// <summary>
    /// Получить игры из стороннего Pgn файла
    /// </summary>
    /// <param name="gameSourse">Источник данных игр в виде pgn множества</param>
    /// <returns></returns>
    public async Task GetAllGamesFromPgn(IPgn gameSourse) // получаем игры из pgn и заранее не знаем откуда именно они пришли
    {
        var allGames = await gameSourse.GetPgnGamesAsync(Name);

        foreach(var pgn in ChessHelper.GetSplittedPGNmass(allGames.Content))
        {
            var game = Game.Create(pgn);
            TryAddGame(game);
        }
    }

    /// <summary>
    /// Проверяет есть ли игры у игрока
    /// </summary>
    /// <returns></returns>
    public bool HaveAnyGames() => games.Any();

    /// <summary>
    /// Фабричный метод создания игроков
    /// </summary>
    /// <param name="name">Логин игрока</param>
    /// <returns></returns>
    public static Player Create(string name) => new(name);
}