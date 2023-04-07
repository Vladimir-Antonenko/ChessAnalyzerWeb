using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using Microsoft.AspNetCore.Http.Extensions;
using ChessAnalyzerApi.UI.ChessTemplateDocument;

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
        private readonly CancellationTokenSource _ctSource;

        // ����� ��� �������� nightQueen111
        public AnalyzeController(ILichess lichess, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _lichess = lichess;
            _logger = logger;
            _ctSource = new();
        }

        /// <summary>
        /// ����� ���� ��� ������
        /// </summary>
        /// <param name="userName">����� �� lichess</param>
        /// <returns></returns>
        [Route("{userName}/FindPlayerGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> FindPlayerGames([FromRoute] string userName)
        {
            var player = await _playerRepository.FindByName(userName);
            if (player is null)
            {
                player = Player.Create(userName);
                _playerRepository.Add(player);
            }
            //AddProgressHandlerEvents(LichessService.processMsgHander); // ���� ������ (���� ��� �� ��������� �������� ��������)
            await player.GetAllGamesFromPgn(_lichess);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            await _playerRepository.Save();
            return Ok(player.HaveAnyGames());
        }

        /// <summary>
        /// ��������� ������ ��� ������
        /// </summary>
        /// <param name="userName">����� ������, ������ �������� ����� ���������������</param>
        /// <param name="precision">�������� �������� ������ ��� ��������� ���� � ������� ���������</param>
        /// <returns></returns>
        [Route("AnalyzeGames/userName={userName}&precision={precision}")]
        [HttpGet]
        public async Task<ActionResult<bool>> AnalyzeGames([FromRoute] string userName, [FromRoute] double precision)
        {
            var player = await _playerRepository.FindByName(userName);

            if (player is null)
                return NotFound(new { message = "����� �� ������" });

            await _analyzeService.RunAnalyzePlayerGames(player, precision, token: _ctSource.Token);

            await _playerRepository.Save(); // ���� ��������� ���� ������� (��� �� ����������� ������� ������ � ����� ������). ���� ������ ������� �� ����� - ����� ������ ���������� (�������� �� ������� �� �������)!
            return Ok();
            // RedirectToAction("RunAnalyzeGames", ); // ��� ���� ��������� ����������� ���� ������ ������������
        }

        /// <summary>
        /// ���������� ������� ����������� html �������� � ����������� ��� ������
        /// </summary>
        /// <param name="userName">����� �� lichess</param>
        /// <param name="numPage">����� �������� ������</param>
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

        /// <summary>
        /// ������ ������� ������
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("CancelAnalyze/userName={userName}")]
        [HttpGet]
        public async Task<ActionResult<bool>> CancelAnalyze([FromRoute] string userName)
        {
            await Task.Run(() => _ctSource.Cancel(true));
            return Ok(true);
        }
    }
}