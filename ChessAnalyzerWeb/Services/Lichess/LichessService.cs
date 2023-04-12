using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;

namespace ChessAnalyzerApi.Services.Lichess;

/// <summary>
/// Сервис взаимодействующий с api сайта Lichess
/// </summary>
public class LichessService : ILichess
{   
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public ChessPlatform? Platform => ChessPlatform.Lichess;

    public LichessService(IMapper mapper, IHttpClientFactory httpClientFactory) // ILogger<LichessService> logger
    {
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient("LichessAPI");
    }

    //////https://lichess.org/api#tag/Opening-Explorer/operation/openingExplorerMasterGame // БД с играми
    ////// http://tablebase.lichess.ovh/standard?fen=4k3/6KP/8/8/8/8/7p/8_w_-_-_0_1 // прям конечная точка

    /// <summary>
    /// Строка для получения игр пользователя (без baseAdress)
    /// </summary>
    /// <param name="login">Логин игрока</param>
    /// <param name="since">С какой даты необходимо получить игры</param>
    /// <param name="until">По какую дату нужны игры</param>
    /// <returns></returns>
    private static string GamesString(string login, DateTime? since, DateTime? until)
    {
        string gameString = @$"games/user/{login}?";
        gameString += since is not null ? $"{nameof(since)}={since.ToUnixTimestamp()}" : string.Empty;
        gameString += until is not null ? $"{nameof(until)}={until.ToUnixTimestamp()}" : string.Empty;

        return gameString;
    }

    /// <summary>
    /// Получить игры пользователя в диапазоне дат {since : until}
    /// </summary>
    /// <param name="login">Логин игрока</param>
    /// <param name="since">С какой даты необходимо получить игры</param>
    /// <param name="until">По какую дату нужны игры</param>
    /// <returns></returns>
    public async Task<Pgn> GetPgnGamesAsync(string login, DateTime? since, DateTime? until)
    {
        LichessPgnModel lichessPgn = new();
        string allGames = string.Empty;

        var requestUri = GamesString(login, since, until);
        var response = await _httpClient.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            allGames = await response.Content.ReadAsStringAsync(); // нужно написать mediaTypeFormatter для "application/x-chess-pgn" и считать по-нормальному не в string
        }

        lichessPgn.Content = allGames; // соответственно это удалить когда будет готов mediaTypeFormatter

        return _mapper.Map<LichessPgnModel, Pgn>(lichessPgn);
    }

    /// <summary>
    /// Получить оценку позиции по заданному fen
    /// </summary>
    /// <param name="fen">Позиция в формате fen строки</param>
    /// <returns></returns>
    public async Task<PositionEvaluation> GetPositionEvaluationAsync(string fen)
    {
        LichessEvaluationModel instance = new();

        var response = await _httpClient.GetAsync($"cloud-eval?fen={fen}&multiPv=1");
        if (response.IsSuccessStatusCode)
        {
            instance = await response.Content.ReadFromJsonAsync<LichessEvaluationModel>();
        }

        return _mapper.Map<LichessEvaluationModel, PositionEvaluation>(instance);
    }

    // попытка создать вшенший движок личесс
    //public async Task CreateEngineAsync()
    //{
    //    EngineCreate engineCreate = new EngineCreate()
    //    {
    //        name = "Stockfish 15",
    //        maxThreads = 4,
    //        maxHash = 2048,
    //        defaultDepth = 20,
    //        variants = new List<string>() { "chess" },
    //        providerSecret = "XXX",
    //        providerData = "strings"
    //    }; ;

    //    var response = await _httpClient.PostAsJsonAsync($"external-engine", engineCreate);
    //    if (response.IsSuccessStatusCode)
    //    {
    //        var instance = await response.Content.ReadFromJsonAsync<dynamic>();
    //    }
    //}
}