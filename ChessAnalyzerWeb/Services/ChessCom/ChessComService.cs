using AutoMapper;
using Domain.Common;
using Domain.GameAggregate;
using ChessAnalyzerApi.Services.ChessCom.Models;

namespace ChessAnalyzerApi.Services.ChessCom;

/// <summary>
/// Документация по chessCom api https://www.chess.com/news/view/published-data-api
/// </summary>
public class ChessComService : IChessCom
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public ChessPlatform? Platform => ChessPlatform.ChessCom;

    public ChessComService(IMapper mapper, IHttpClientFactory httpClientFactory) // ILogger<LichessService> logger
    {
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient("ChessComAPI");
    }

    /// <summary>
    /// Получает все ссылки на архивные части игр игрока в формате pgn
    /// </summary>
    /// <param name="login">Логин игрока на ChessCom</param>
    /// <returns></returns>
    private async Task<GameReferences> GetArhivesGameReferences(string login)
    {
        GameReferences gameRefs = new();

        var response = await _httpClient.GetAsync($"pub/player/{login}/games/archives");
        if (response.IsSuccessStatusCode)
        {
            gameRefs = await response.Content.ReadFromJsonAsync<GameReferences>();
        }

        return gameRefs;
    }

    /// <summary>
    /// Получить игры пользователя в диапазоне дат {since : until}
    /// </summary>
    /// <param name="login">Логин игрока на ChessCom</param>
    /// <param name="since">Дата "с"</param>
    /// <param name="until">Дата "по"</param>
    /// <returns></returns>
    public async Task<Pgn> GetPgnGamesAsync(string login, DateTime? since, DateTime? until)
    {
        // ChessCom позволяет загрузить только по году и месяцу и возвращает по каждой ссылке свой pgn

        ChessComPgnModel pgnModel = new();
        List<PartArchivePgn> pgnParts = new();
        var dateRange = DateRange.Create(start: since, end: until);

        var gameRefs = await GetArhivesGameReferences(login);

        var rangeRefs = gameRefs.archives.Where(x => dateRange.InRange(x.Split("/games/").LastOrDefault())); 
                                              
        foreach (var gameRef in rangeRefs)
        {
            var part = await GetPartArchiveGames(gameRef);
            pgnParts.Add(part);
        }

        pgnModel.Content = string.Join("\n\n", pgnParts);

        return _mapper.Map<ChessComPgnModel, Pgn>(pgnModel);
    }

    /// <summary>
    /// Вернет по ссылке часть архима игр в формате pgn
    /// </summary>
    /// <param name="url">Ссылка на скачиваемую часть игр в формате pgn</param>
    /// <returns></returns>
    private async Task<PartArchivePgn> GetPartArchiveGames(string url)
    {
        PartArchivePgn partPgn = new();
        var pgnUrl = $"{url.Replace("https://api.chess.com/", "")}/pgn"; //pub/player/{username}/games/{YYYY}/{MM}/pgn

        var response = await _httpClient.GetAsync(pgnUrl);
        if (response.IsSuccessStatusCode)
        {
            partPgn.PartContent = await response.Content.ReadAsStringAsync();
        }

        return partPgn;
    }
}