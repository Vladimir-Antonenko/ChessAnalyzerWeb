using Domain.GameAggregate;
using ChessAnalyzerApi.Models;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Services.Analyze;

namespace ChessAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IAnalyzeService _analyzeService;
        private readonly ILogger<AnalyzeController> _logger;
        private readonly Func<ChessPlatform, IPgn> _pgnServiceAccessor; // ��� �������� ���������

        // ����� ��� �������� nightQueen111
        public AnalyzeController(Func<ChessPlatform, IPgn> pgnServiceAccessor, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _pgnServiceAccessor = pgnServiceAccessor;
            _logger = logger;
        }

        /// <summary>
        /// ����� ��� ������
        /// </summary>
        /// <param name="userName">����� �� lichess</param>
        /// <param name="since">���� ������ ��� "�"</param>
        /// <param name="until">���� ������ ��� "��"</param>
        /// <returns></returns>
        [Route("FindPlayerGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> FindPlayerGames([FromBody] FindPlayerGamesModel findModel)
        {
            var player = await _playerRepository.FindByName(findModel.userName);
            if (player is null)
            {
                player = Player.Create(findModel.userName);
                _playerRepository.Add(player);
            }

            var pgnService = _pgnServiceAccessor(findModel.platform); // �������� ��������������� ������ ��� �������� ���

            //AddProgressHandlerEvents(LichessService.processMsgHander); // ���� ������ (���� ��� �� ��������� �������� ��������)
            await player.GetGamesFromPgn(pgnService, findModel.since, findModel.until);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            // �� ������� ������� (��������� ����������)
            ////private void HttpReceiveProgressEvent(object? sender, HttpProgressEventArgs e)
            ////{
            ////    ProgressBarValue = e.ProgressPercentage;// ��������� 
            ////    InfoLoad = $"���������: {e.BytesTransferred / 1024} ����� / {e.TotalBytes / 1024} �����"; //������� � ����� ���������� � �������� ��������
            ////}

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
            var player = await _playerRepository.FindByNameOnPlatform(infoModel.userName, infoModel.platform);

            if (player is null)
                return NotFound(new { message = $"�� ������ ����� � ������, ������� ������� ��������� {Enum.GetName(infoModel.platform)}!" });

            await _analyzeService.RunAnalyzePlayerGames(player, infoModel.precision, cancelToken);

            await _playerRepository.Save(); // ���� ��������� ���� ������� (��� �� ����������� ������� ������ � ����� ������). ���� ������ ������� �� ����� - ����� ������ ���������� (�������� �� ������� �� �������)!
            
            return Ok();
        }

        [Route("GetAvailablePlatforms")]
        [HttpGet]
        public async Task<ActionResult<Dictionary<int, string>>> GetAvailablePlatforms()
        {
            var platforms = await Task.Run(() =>
                Enum.GetValues(typeof(ChessPlatform))
               .Cast<ChessPlatform>()
               .ToDictionary(t => (int)t, t => t.ToString()) 
            );

            // Array platforms = await Task.Run(() => Enum.GetValues(typeof(ChessPlatform))); //Enum.GetValues(typeof(ChessPlatform)).Cast<ChessPlatform>().ToList();
            // Enum.GetNames(typeof(Enumnum));
            return platforms;
        }
    }
}