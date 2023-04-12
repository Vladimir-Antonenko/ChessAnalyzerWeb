using AutoMapper;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.Services.ChessCom;

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

    public Task<Pgn> GetPgnGamesAsync(string login, DateTime? since, DateTime? until)
    {
        throw new NotImplementedException();

        // Позволяет загрузить только по году и месяцу и возвращает по каждой ссылке свой pgn (надо в цикле грузить их и склеивать в один)
        //pub/player/jora_barygin/games/2016/11/pgn
        //pub/player/{username}/games/{YYYY}/{MM}/pgn

        // Так на личессе загружаю
        //LichessPgnModel lichessPgn = new();
        //string allGames = string.Empty;

        //var requestUri = GamesString(login, since, until);
        //var response = await _httpClient.GetAsync(requestUri);
        //if (response.IsSuccessStatusCode)
        //{
        //    allGames = await response.Content.ReadAsStringAsync(); // нужно написать mediaTypeFormatter для "application/x-chess-pgn" и считать по-нормальному не в string
        //}

        //lichessPgn.Content = allGames; // соответственно это удалить когда будет готов mediaTypeFormatter

        //return _mapper.Map<LichessPgnModel, Pgn>(lichessPgn);
    }
}