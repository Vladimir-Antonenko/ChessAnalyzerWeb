using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;

namespace ChessAnalyzerApi.ExternalApi.Lichess;

internal class LichessService : IPositionEvaluation, IPgn
{   
    private readonly IMapper mapper;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly CancellationToken token;

    internal string Login { get; private set; }

    private LichessService(IMapper mapper, IHttpClientFactory httpClient, string loginLichess, CancellationToken token)
    {
        this.mapper = mapper;
        this.httpClientFactory = httpClient;
        Login = loginLichess;
        this.token = token;
    }

    private string EvaluateString(string fen, int multiPv = 1) => @$"https://lichess.org/api/cloud-eval?fen={fen}&multiPv={multiPv}"; // строка для получения оценки с lichess
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
        var client = httpClientFactory.CreateClient();

        using (var response = await client.GetAsync(uri, token))
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
        var client = httpClientFactory.CreateClient();

        using (var response = await client.GetAsync(EvaluateString(fen), token))
        {
            if (response.IsSuccessStatusCode)
            {
                instance = await response.Content.ReadFromJsonAsync<LichessEvaluationModel>();
            }
        }

        return mapper.Map<LichessEvaluationModel, PositionEvaluation>(instance);
    }

    public static LichessService Create(IMapper mapper, IHttpClientFactory httpClient, string loginLichess, CancellationToken token) => new(mapper, httpClient, loginLichess, token); // фабрика
}
