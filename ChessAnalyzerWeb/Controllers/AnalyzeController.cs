using ChessAnalyzerApi.Services.Lichess;
using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILichess _lichess;
        private readonly ILogger<AnalyzeController> _logger;

        public AnalyzeController(ILichess lichess, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger) //IPlayerRepository playerRepository, 
        {
            _playerRepository = playerRepository;
            _lichess = lichess;
            _logger = logger;
        }

        [HttpGet(Name = "FindPlayerGames")]
        public async Task<bool> FindPlayerGames(string login)
        {
            var player = await _playerRepository.FindByName(login);
            player ??= Player.Create(login);
            //AddProgressHandlerEvents(LichessService.processMsgHander); // скорее всего 
            await player.GetAllGamesFromPgn(_lichess);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);
            return player.HaveAnyGames();
        }
    }
}