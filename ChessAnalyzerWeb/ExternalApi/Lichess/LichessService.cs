using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;

namespace ChessAnalyzerApi.ExternalApi.Lichess;

internal class LichessService : IPositionEvaluation, IPgn
{   
    private readonly IMapper mapper;
    private readonly HttpClient httpClient;
    private readonly CancellationToken token;


    internal string Login { get; private set; }

    private LichessService(IMapper mapper, HttpClient httpClient, string loginLichess, CancellationToken token)
    {
        this.mapper = mapper;
        this.httpClient = httpClient;
        Login = loginLichess;
        this.token = token;
    }

    ////public WeatherForecastController(ILogger<WeatherForecastController> logger, IHubContext<ChatHub> hubContext, HttpClient http)
    ////{
    ////    _hubContext = hubContext;
    ////    _logger = logger;
    ////    _httpClient = http;
    ////    _httpClient.BaseAddress = new Uri("http://something.com/api/"); // пример задания базовой строки (должна быть / в конце) -- такую штуку можно задать сразу в файле program!
    ////    var response = await _httpClient.GetAsync("resource/7"); // а у адреса не должно быть
    ////    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token"); // тоже у клиента задать токен сразу
    ////}

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

    // !!!!!!!!!!!! Попробовать укоротить https://lichess.org/api/ и проверить работает или нет!
    private string GamesString(DateTime since = default, DateTime until = default) // строка для получения игр пользователя
    {
        string gameString = @$"https://lichess.org/api/games/user/{Login}?";
        gameString += since != default ? $"{nameof(since)}={since.ToUnixTimestamp()}" : string.Empty;
        gameString += until != default ? $"{nameof(until)}={until.ToUnixTimestamp()}" : string.Empty;

        return gameString;
    }

    private async Task<Pgn> GetLichessGamesAsync(Uri uri) // получить игры пользователя зная строку фильтра параметров
    {
        LichessPgnModel lichessPgn = new();
        string allGames = string.Empty;
 
        using (var response = await httpClient.GetAsync(uri, token))
        {
            if (response.IsSuccessStatusCode)
            {
                allGames = await response.Content.ReadAsStringAsync(token);   // нужно написать mediaTypeFormatter для "application/x-chess-pgn" и считать по-нормальному не в string
            }
        }

        lichessPgn.Content = allGames; // соответственно это удалить когда будет готов mediaTypeFormatter

        return mapper.Map<LichessPgnModel, Pgn>(lichessPgn);
    }

    public async Task<Pgn> GetAllPgnGamesAsync() // получить все игры пользователя
    {
        Uri uri = new(GamesString()); //ссылка на файл
        return await GetLichessGamesAsync(uri);
    }

    public async Task<Pgn> GetLichessGamesByDatesAsync(DateTime since = default, DateTime until = default) // получить игры пользователя в диапазоне дат {since : until}
    {
        Uri uri = new(GamesString(since, until)); //ссылка на файл
        return await GetLichessGamesAsync(uri);
    }

    public async Task<PositionEvaluation> GetPositionEvaluationAsync(string fen) // получить оценку позиции по заданному fen
    {
        LichessEvaluationModel instance = new();

        using (var response = await httpClient.GetAsync($"cloud-eval?fen={fen}&multiPv=1", token))
        {
            if (response.IsSuccessStatusCode)
            {
                instance = await response.Content.ReadFromJsonAsync<LichessEvaluationModel>();
            }
        }

        return mapper.Map<LichessEvaluationModel, PositionEvaluation>(instance);
    }

    public static LichessService Create(IMapper mapper, HttpClient httpClient, string loginLichess, CancellationToken token) => new(mapper, httpClient, loginLichess, token); // фабрика
}
