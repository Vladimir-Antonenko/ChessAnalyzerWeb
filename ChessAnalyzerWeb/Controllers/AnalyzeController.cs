using ChessAnalyzerApi.Services.Lichess;
using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api")]
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

        [Route("{userName}/FindPlayerGames")]
        [HttpPost]
        public async Task<bool> FindPlayerGames([FromRoute] string userName) //    // логин nightQueen111
        {
            var player = await _playerRepository.FindByName(userName);
            player ??= Player.Create(userName);
            //AddProgressHandlerEvents(LichessService.processMsgHander); // скорее всего 
            await player.GetAllGamesFromPgn(_lichess);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);
            return player.HaveAnyGames();
        }
    }
}