using Domain.Extensions;
using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;
using ChessAnalyzerApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.Extensions;
using ChessAnalyzerApi.TemplateMistakesPage;

namespace ChessAnalyzerApi.Controllers.Mistakes
{
    [ApiController]
    [Route("api")]
    public class MistakesController : ControllerBase
    {
        private const int STANDART_PAGE_SIZE = 6;
        private static readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<MistakesController> _logger;
        private readonly IMemoryCache _cache;

        public MistakesController(IPlayerRepository playerRepository, ILogger<MistakesController> logger, IMemoryCacheService cacheService)
        {
            _playerRepository = playerRepository;
            _logger = logger;
            _cache = cacheService.Cache;
        }

        /// <summary>
        /// Отправляет клиенту формируемую html страницу с диаграммами его ошибок
        /// </summary>
        /// <param name="userName">Логин игрока на выбранной платформе</param>
        /// <param name="platform">Шахматная платформа</param>
        /// <param name="numPage">Номер страницы ошибок</param>
        /// <returns></returns>
        [Route("{userName}/{platform}/Mistakes/{numPage:int:min(1)}")]
        [HttpGet]
        public async Task<ContentResult> GetPagePlayerMistakes([FromRoute] string userName, [FromRoute] ChessPlatform platform, [FromRoute] int numPage)
        {
            string htmlPageCacheKey = $"{platform}{userName}";

            _logger.LogInformation("Пытаюсь извлечь страницу ошибок пользователя {0} на платформе {1} из кэша.", userName, platform);

            if (_cache.TryGetValue(htmlPageCacheKey, out string? html))
            {
                _logger.LogInformation("Страница найдена в кэше.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync(); // с семафором для того чтобы управлять одновременным доступом к кэшу в памяти
                    if (_cache.TryGetValue(htmlPageCacheKey, out html))
                    {
                        _logger.LogInformation("Страница ошибок найдена в кэше.");
                    }
                    else
                    {
                        _logger.LogInformation("Страница не найдена в кэше. Создаю новую.");


                        var partMistakes = await _playerRepository.GetMistakesWithPagination(userName, platform, numPage, pageSize: STANDART_PAGE_SIZE);

                        if (partMistakes.Anybody())
                        {
                            var mistakesUrl = HttpContext.Request.GetDisplayUrl();
                            html = PageTemplate.Create(partMistakes, numPage, requestUrl: mistakesUrl).GetHtml();
                        }

                        // с размерами надо поиграть
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                                .SetPriority(CacheItemPriority.Normal);
                        //.SetSize(1024);

                        _cache.Set(htmlPageCacheKey, html, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return Content(html ?? string.Empty, "text/html");
        }
    }
}