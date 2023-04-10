using Domain.GameAggregate;
using ChessAnalyzerApi.Models;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IAnalyzeService _analyzeService;
        private readonly ILichess _lichess;
        private readonly ILogger<AnalyzeController> _logger;

        // ����� ��� �������� nightQueen111
        public AnalyzeController(ILichess lichess, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _lichess = lichess;
            _logger = logger;
        }

        /// <summary>
        /// ����� ��� ������
        /// </summary>
        /// <param name="userName">����� �� lichess</param>
        /// <param name="since">���� ������ ��� "�"</param>
        /// <param name="until">���� ������ ��� "��"</param>
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
            //AddProgressHandlerEvents(LichessService.processMsgHander); // ���� ������ (���� ��� �� ��������� �������� ��������)
            await player.GetGamesFromPgn(_lichess, since, until);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            await _playerRepository.Save();
            return Ok(player.HaveAnyGames());
        }

        /// <summary>
        /// ��������� ������ ��� ������
        /// </summary>
        /// <param name="infoModel">������ ����������� ��� ������ �������</param>
        /// <param name="cancelToken">����� ������ ���������� �������� (�� �� HttpContext.RequestAborted)</param>
        /// <returns></returns>
        [Route("AnalyzeGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> AnalyzeGames([FromBody] AnalyzeInfoModel infoModel, CancellationToken cancelToken)
        {
            var player = await _playerRepository.FindByName(infoModel.userName);

            if (player is null)
                return NotFound(new { message = "����� �� ������" });

            await _analyzeService.RunAnalyzePlayerGames(player, infoModel.precision, cancelToken);

            await _playerRepository.Save(); // ���� ��������� ���� ������� (��� �� ����������� ������� ������ � ����� ������). ���� ������ ������� �� ����� - ����� ������ ���������� (�������� �� ������� �� �������)!
            return Ok();
            // RedirectToAction("RunAnalyzeGames", ); // ��� ���� ��������� ����������� ���� ������ ������������
        }
    }
}