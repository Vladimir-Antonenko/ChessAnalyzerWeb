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
        private readonly Func<ChessPlatform, IPgn> _pgnServiceAccessor; // для паттерна стратегия

        // логин для проверки nightQueen111
        public AnalyzeController(Func<ChessPlatform, IPgn> pgnServiceAccessor, IAnalyzeService analyzeService, IPlayerRepository playerRepository, ILogger<AnalyzeController> logger)
        {
            _playerRepository = playerRepository;
            _analyzeService = analyzeService;
            _pgnServiceAccessor = pgnServiceAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Поиск игр игрока
        /// </summary>
        /// <param name="userName">Логин на lichess</param>
        /// <param name="since">Дата поиска игр "с"</param>
        /// <param name="until">Дата поиска игр "по"</param>
        /// <returns></returns>
        [Route("FindPlayerGames")]
        [HttpPost]
        public async Task<ActionResult<bool>> FindPlayerGames([FromBody] FindPlayerGamesModel findModel, [FromQuery] DateTime since = default, [FromQuery] DateTime until = default)
        {
            var player = await _playerRepository.FindByName(findModel.userName);
            if (player is null)
            {
                player = Player.Create(findModel.userName);
                _playerRepository.Add(player);
            }

            var pgnService = _pgnServiceAccessor(findModel.platform); // получаем соответствующий сервис для загрузки игр

            //AddProgressHandlerEvents(LichessService.processMsgHander); // было раньше (пока ещё не прикрутил прогресс загрузки)
            await player.GetGamesFromPgn(pgnService, since, until);
            //RemoveProgressHandlerEvents(LichessService.processMsgHander);

            // Из старого проекта (наподобие переделать)
            ////private void HttpReceiveProgressEvent(object? sender, HttpProgressEventArgs e)
            ////{
            ////    ProgressBarValue = e.ProgressPercentage;// заполняем 
            ////    InfoLoad = $"Загружено: {e.BytesTransferred / 1024} Кбайт / {e.TotalBytes / 1024} Кбайт"; //Выводим в лейбл информацию о процессе загрузки
            ////}

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
            var player = await _playerRepository.FindByName(infoModel.userName); // тут использовать FindByNameOnPlatform и ннадо добавить в модель анализа тип платформы

            if (player is null)
                return NotFound(new { message = "Логин не найден" });

            await _analyzeService.RunAnalyzePlayerGames(player, infoModel.precision, cancelToken); // и тут анализ соответствующих игр

            await _playerRepository.Save(); // Пока прикручен хард стокфиш (так мы гарантируем наличие оценки в любом случае). Если оценки позиции не будет - может выдать исключение (например не найдена на личессе)!
            return Ok();
            // RedirectToAction("RunAnalyzeGames", ); // тут надо параметры перечислить если вообще использовать
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