using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.UI.ChessTemplateDocument;
using Infrastructure.Repositories;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyzeController : ControllerBase
    {
        private const int STANDART_PAGE_SIZE = 6;
        private readonly IPlayerRepository _playerRepository;
        private readonly IAnalyzeService _analyzeService;
        private readonly ILichess _lichess;
        private readonly ILogger<AnalyzeController> _logger;

        public AnalyzeController(ILichess lichess, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger) //IPlayerRepository playerRepository, 
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _lichess = lichess;
            _logger = logger;
        }

        [Route("{userName}/FindPlayerGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> FindPlayerGames([FromRoute] string userName) //    // логин nightQueen111
        {
            var player = await _playerRepository.FindByName(userName);
            if (player is null)
            {
                player = Player.Create(userName);
                _playerRepository.Add(player);
            }
            //AddProgressHandlerEvents(LichessService.processMsgHander); // скорее всего 
            await player.GetAllGamesFromPgn(_lichess);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            await _playerRepository.Save();
            return Ok(player.HaveAnyGames());
        }

        // тщательно пересмотреть внутренность контроллера!!!
        [Route("AnalyzeGames/userName={userName}&precision={precision}")]
        [HttpGet]
        public async Task<ActionResult<bool>> AnalyzeGames([FromRoute] string userName, [FromRoute] double precision)
        {
            var player = await _playerRepository.FindByName(userName);

            if (player is null)
                return NotFound(new { message = "Логин не найден" });

            await _analyzeService.RunAnalyzePlayerGames(player, precision);

            await _playerRepository.Save(); // пока прикручен хард стокфиш мы гарантируем наличие оценки в любом случае. Если оценки позиции не будет - может выдать исключение! Если например от личесса фен придет null например
            return Ok();
            // RedirectToAction("RunAnalyzeGames", ); // тут надо параметры перечислить если вообще использовать
        }

        [Route("{userName:max(50)}/Lichess/Mistakes/{numPage:int:min(1)}")]
        [HttpGet]
        public async Task<ContentResult> GetPagePlayerMistakes([FromRoute] string userName, [FromRoute] int numPage)
        {
            var partMistakes = await _playerRepository.GetMistakesWithPagination(userName, numPage, pageSize: STANDART_PAGE_SIZE);

            string html = string.Empty;
            if (partMistakes.Anybody())
            {
                html = PageTemplate.Create(partMistakes, numPage, linkPattern: $"{userName}/Lichess/Mistakes/").GetHtml();
            }

            return Content(html, "text/html");

            // return Redirect("/Home/Index");
        }

    }
}