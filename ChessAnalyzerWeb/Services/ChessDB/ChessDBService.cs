using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Services.ChessDB.Models;
using ChessAnalyzerApi.ExtensionsChessAnalyzerApi.Extensions;

namespace ChessAnalyzerApi.Services.ChessDB
{
    /// <summary>
    /// Инструкция по работе с api от ChessDB https://www.chessdb.cn/cloudbookc_api_en.html
    /// </summary>
    public class ChessDBService : IChessDBService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChessDBService> _logger;

        public ChessDBService(IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<ChessDBService> logger)
        {
            _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient("ChessDB");
            _logger = logger;
        }

        /// <summary>
        /// Получение оценки позиции с ресурса ChessDB
        /// </summary>
        /// <param name="fen">Позиция в формате fen строки</param>
        /// <returns></returns>
        public async Task<PositionEvaluation> GetPositionEvaluationAsync(string fen)
        {
            QueryPvModel instance = new();

            var response = await _httpClient.GetAsync($"cdb.php?action=querypv&board={fen}&json=1");
            if (response.IsSuccessStatusCode)
            {
                instance = await response.Content.ReadFromJsonAsync<QueryPvModel>();
            }

            return _mapper.Map<QueryPvModel, PositionEvaluation>(instance, ("Fen", fen)); // дополнительно прокидываю fen в итоговую модель
        }
    }
}