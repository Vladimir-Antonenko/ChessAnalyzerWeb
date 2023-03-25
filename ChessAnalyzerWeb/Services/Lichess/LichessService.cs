using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;

namespace ChessAnalyzerApi.Services.Lichess;

public class LichessService : ILichess
{   
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public LichessService(IMapper mapper, HttpClient httpClient) // ILogger<LichessService> logger
    {
        _mapper = mapper;
        _httpClient = httpClient;
    }
    //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token"); // тоже у клиента задать токен сразу
    ////// про токен надо подумать
    //////https://lichess.org/api#tag/Opening-Explorer/operation/openingExplorerMasterGame // БД с играми
    ////// http://tablebase.lichess.ovh/standard?fen=4k3/6KP/8/8/8/8/7p/8_w_-_-_0_1 // прям конечная точка
    ////// deserialize
    //////public async Task<UserModel> GetUserAsync(int userId)
    //////{
    //////    var result = await _httpClient.GetAsync($"api/users/{userId}");
    //////    result.EnsureSuccessStatusCode();
    //////    var response = await result.Content.ReadAsStringAsync();
    //////    return DeserializeResult<UserModel>(response);
    //////}

    /// <summary>
    /// Строка для получения игр пользователя (без baseAdress)
    /// </summary>
    /// <param name="login"></param>
    /// <param name="since"></param>
    /// <param name="until"></param>
    /// <returns></returns>
    private string GamesString(string login, DateTime since = default, DateTime until = default)
    {
        string gameString = @$"games/user/{login}?";
        gameString += since != default ? $"{nameof(since)}={since.ToUnixTimestamp()}" : string.Empty;
        gameString += until != default ? $"{nameof(until)}={until.ToUnixTimestamp()}" : string.Empty;

        return gameString;
    }

    /// <summary>
    /// Получить игры пользователя зная строку фильтра параметров
    /// </summary>
    /// <param name="login"></param>
    /// <param name="since"></param>
    /// <param name="until"></param>
    /// <returns></returns>
    private async Task<Pgn> GetLichessGamesAsync(string login, DateTime since = default, DateTime until = default)
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
    /// <param name="fen"></param>
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

    /// <summary>
    /// Получить все игры пользователя в диапазоне дат {since : until}
    /// </summary>
    /// <param name="login"></param>
    /// <param name="since"></param>
    /// <param name="until"></param>
    /// <returns></returns>
    public async Task<Pgn> GetPgnGamesAsync(string login, DateTime since = default, DateTime until = default)
        => await GetLichessGamesAsync(login, since, until);
}