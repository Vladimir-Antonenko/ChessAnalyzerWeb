using Domain.Extensions;
using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using ChessAnalyzerApi.TemplateMistakesPage;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class MistakesController : ControllerBase
    {
        private const int STANDART_PAGE_SIZE = 6;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<AnalyzeController> _logger;

        public MistakesController(IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
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