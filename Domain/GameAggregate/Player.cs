﻿namespace Domain.GameAggregate;

public class Player
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Game> games = new();
    public IReadOnlyCollection<Game> Games => games;

    //private readonly List<Position> mistakes = new();
    //public IReadOnlyCollection<Position> Mistakes => mistakes;
    // для ef core
    private Player() { } // для ef core

    private Player(string userName)
    {
        Name = userName;
    }

    public bool IsContainedThisGame(Game game) => games.Any(x => x.Pgn.Equals(game.Pgn));

    public Color GetPlayerColorInGame(Game game) =>
        game.WhiteGamer.Equals(this.Name) ?
            Color.White : game.BlackGamer.Equals(this.Name) ?
                Color.Black : throw new InvalidDataException($"Игра {game.Pgn}\n не принадлежит игроку");

    public void AddGames(IEnumerable<Game> games)
    {
        foreach (var game in games)
            TryAddGame(game);
    }

    public bool TryAddGame(Game game)
    {
        if (!IsContainedThisGame(game) && game.HaveAnyPositions())
        {
            games.Add(game);
            return true;
        }    
        
        return false;
    }

    public void AddToMistakes(Position position)
    {
        if (!position.IsEvaluated())
            throw new InvalidDataException("Позиция без оценки не может считаться ошибкой");
        else
            position.SetPositionIsMistake(true);
    }

    public async Task GetAllGamesFromPgn(IPgn gameSourse) // получаем игры из pgn и заранее не знаем откуда именно они пришли
    {
        var allGames = await gameSourse.GetPgnGamesAsync(Name);

        foreach(var pgn in ChessHelper.GetSplittedPGNmass(allGames.Content))
        {
            var game = Game.Create(pgn);
            TryAddGame(game);
        }
    }

    public bool HaveAnyGames() => games.Any();

    public static Player Create(string name) => new(name);
}