using Domain.GameAggregate;
using ChessAnalyzerApi.Models;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using Microsoft.AspNetCore.Http.Extensions;
using ChessAnalyzerApi.TemplateMistakesPage;

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

        // логин для проверки nightQueen111
        public AnalyzeController(ILichess lichess, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _lichess = lichess;
            _logger = logger;
        }

        /// <summary>
        /// Поиск игр игрока
        /// </summary>
        /// <param name="userName">Логин на lichess</param>
        /// <param name="since">Дата поиска игр "с"</param>
        /// <param name="until">Дата поиска игр "по"</param>
        /// <returns></returns>
        [Route("{userName}/FindPlayerGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> FindPlayerGames([FromRoute] string userName, [FromQuery] DateTime since = default, [FromQuery] DateTime until = default)
        {
            var player = await _playerRepository.FindByName(userName);
            if (player is null)
            {
                player = Player.Create(userName);
                _playerRepository.Add(player);
            }
            //AddProgressHandlerEvents(LichessService.processMsgHander); // было раньше (пока ещё не прикрутил прогресс загрузки)
            await player.GetGamesFromPgn(_lichess, since, until);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            await _playerRepository.Save();
            return Ok(player.HaveAnyGames());
        }

        /// <summary>
        /// Запускает анализ игр игрока
        /// </summary>
        /// <param name="infoModel">Данные необходимые для начала анализа</param>
        /// <param name="cancelToken">Токен отмены выполнения операции (он же HttpContext.RequestAborted)</param>
        /// <returns></returns>
        [Route("AnalyzeGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> AnalyzeGames([FromBody] AnalyzeInfoModel infoModel, CancellationToken cancelToken)
        {
            var player = await _playerRepository.FindByName(infoModel.userName);

            if (player is null)
                return NotFound(new { message = "Логин не найден" });

            await _analyzeService.RunAnalyzePlayerGames(player, infoModel.precision, cancelToken);

            await _playerRepository.Save(); // Пока прикручен хард стокфиш (так мы гарантируем наличие оценки в любом случае). Если оценки позиции не будет - может выдать исключение (например не найдена на личессе)!
            return Ok();
            // RedirectToAction("RunAnalyzeGames", ); // тут надо параметры перечислить если вообще использовать
        }

        /// <summary>
        /// Отправляет клиенту формируемую html страницу с диаграммами его ошибок
        /// </summary>
        /// <param name="userName">Логин на lichess</param>
        /// <param name="numPage">Номер страницы ошибок</param>
        /// <returns></returns>
        [Route("{userName}/Lichess/Mistakes/{numPage:int:min(1)}")]
        [HttpGet]
        public async Task<ContentResult> GetPagePlayerMistakes([FromRoute] string userName, [FromRoute] int numPage)
        {
            var partMistakes = await _playerRepository.GetMistakesWithPagination(userName, numPage, pageSize: STANDART_PAGE_SIZE);

            string html = string.Empty;
            if (partMistakes.Anybody())
            {
                var mistakesUrl = HttpContext.Request.GetDisplayUrl();
                html = PageTemplate.Create(partMistakes, numPage, requestUrl: mistakesUrl).GetHtml();
            }

            return Content(html, "text/html");

            // return Redirect("/Home/Index");
        }
    }
}