using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Services.ChessDB.Models;
using ChessAnalyzerApi.ExtensionsChessAnalyzerApi.Extensions;

namespace ChessAnalyzerApi.Services.ChessDB
{
    public class ChessDBService : IChessDBService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChessDBService> _logger;

        /// <summary>
        /// Инструкция по работе с api от ChessDB https://www.chessdb.cn/cloudbookc_api_en.html
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public ChessDBService(IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<ChessDBService> logger)
        {
            _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient("ChessDB");
            _logger = logger;
        }

        /// <summary>
        /// Получение оценки с ресурса ChessDB
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public async Task<PositionEvaluation> GetPositionEvaluationAsync(string fen)
        {
            QueryPvModel instance = new();

            var response = await _httpClient.GetAsync($"cdb.php?action=querypv&board={fen}&json=1");
            if (response.IsSuccessStatusCode)
            {
                instance = await response.Content.ReadFromJsonAsync<QueryPvModel>();
            }
            // проверить когда позиции в базе нет что возвращает успешно 
            // if the position is unknown, "unknown" is returned

            // нужно как-то докинуть fen в PositionEvaluation!! у него приватный сеттер!
            return _mapper.Map<QueryPvModel, PositionEvaluation>(instance, ("Fen", fen)); //(nameof(fen), fen)
            // return _mapper.Map<QueryPvModel, PositionEvaluation>(instance, opt => opt.BeforeMap((src, dest) => dest.Fen = fen));
        }
    }
}