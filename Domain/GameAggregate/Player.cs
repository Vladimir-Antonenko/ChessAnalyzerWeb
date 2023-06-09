﻿using Domain.Extensions;

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
        game.WhiteGamer.ToLower().Equals(this.Name.ToLower()) ?
            Color.White : game.BlackGamer.ToLower().Equals(this.Name.ToLower()) ?
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

    /// <summary>
    /// Получить игры из стороннего Pgn файла
    /// </summary>
    /// <param name="gameSourse">Источник данных игр в виде pgn множества</param>
    /// <param name="since">Дата "с"</param>
    /// <param name="until">Дата "по"</param>
    /// <returns></returns>
    public async Task GetGamesFromPgn(IPgn gameSourse, DateTime? since, DateTime? until) // получаем игры из pgn и заранее не знаем откуда именно они пришли
    {
        var loadedGames = await gameSourse.GetPgnGamesAsync(Name, since, until);

        foreach(var pgn in ChessHelper.GetSplittedPGNmass(loadedGames.Content))
        {
            var game = Game.Create(pgn, gameSourse.Platform);
            TryAddGame(game);
        }
    }

    /// <summary>
    /// Проверяет есть ли у игрока игры на выбранной платформе
    /// </summary>
    /// <param name="since">Дата "с"</param>
    /// <param name="until">Дата "по"</param>
    /// <returns></returns>
    public bool HaveAnyGamesOnPlatform(ChessPlatform chessPlatform, DateTime? since, DateTime? until)
    {
        var dateSince = since ?? DateTime.MinValue;
        var dateUntil = until ?? DateTime.MaxValue;

        return games.Any(g => g.Platform.Equals(chessPlatform) && g.DateGame.InRange(dateSince, dateUntil));
    }
       
    /// <summary>
    /// Фабричный метод создания игроков
    /// </summary>
    /// <param name="name">Логин игрока</param>
    /// <returns></returns>
    public static Player Create(string name) => new(name);
}