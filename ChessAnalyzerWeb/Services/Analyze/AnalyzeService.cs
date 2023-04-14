using Domain.Extensions;
using Domain.GameAggregate;
using ChessAnalyzerApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Position = Domain.GameAggregate.Position;

namespace ChessAnalyzerApi.Services.Analyze;

/// <summary>
/// Сервис анализа позиций игрока
/// </summary>
public class AnalyzerService : IAnalyzeService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IEnumerable<IPositionEvaluation> _EvaluationServices;
    private readonly ILogger<AnalyzerService> _logger;

    public AnalyzerService(IEnumerable<IPositionEvaluation> EvaluationServices, IHubContext<NotificationHub> hubContext, ILogger<AnalyzerService> logger)
    {
        _EvaluationServices = EvaluationServices ?? Enumerable.Empty<IPositionEvaluation>();
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Процедура анализа позиции
    /// </summary>
    /// <param name="position">Анализируемая позиции</param>
    /// <returns></returns>
    private async Task AnalyzePosition(Position position)
    {
        foreach (var evaluationService in _EvaluationServices)
        {
            if (!position.IsEvaluated()) // оценка не найдена?
            {
                await position.GetPositionEvaluation(evaluationService);
            }
        }
    }

    /// <summary>
    /// Отправка клиенту уведомлений о прогрессе анализа на основе вебсокета
    /// </summary>
    /// <param name="playerName">Имя игрока</param>
    /// <param name="nGame">Номер анализируемой игры</param>
    /// <param name="nMove">Номер хода</param>
    /// <param name="token">Отмен отмены</param>
    /// <returns></returns>
    private async Task SendProcessNotification(string playerName, int nGame, int nMove, CancellationToken token)
    {
        await _hubContext.Clients
            .Group(playerName)
            .SendAsync(method: "Notification",
                       $"Игра номер {nGame + 1}. Анализирую ход {Math.Ceiling((double)nMove / 2)}",
                        cancellationToken: token);
    }

    /// <summary>
    /// Проверка имеются ли сервисы способные оценить позицию
    /// </summary>
    /// <returns></returns>
    public bool HaveAnyEvaluationServises() => _EvaluationServices.Any();

    /// <summary>
    /// Запускаем анализ игр игрока
    /// </summary>
    /// <param name="player">Игрок</param>
    /// <param name="mistakePrecision">Точность перепада оценки</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    public async Task RunAnalyzePlayerGames(Player player, double mistakePrecision, CancellationToken token = default)
    {
        if (HaveAnyEvaluationServises())
        {
            foreach (var (game, n) in player.Games.WithIndex())
            {
                if (game.HaveAnyPositions()) // позиции есть (отпарсили данную игру успешно)
                {
                    var colorInGame = player.GetPlayerColorInGame(game); // цвет player в этой игре
                    Position prevPosition = game.Positions.First();
                    prevPosition.SetEvaluation(0.0d); // у первой оценка 0

                    foreach (var (position, i) in game.Positions.Skip(1).WithIndex()) // для всех позиций начиная со второй (пропуская первую) получаю оценку
                    {
                        if (token.IsCancellationRequested)  // проверяем наличие сигнала отмены задачи
                        {
                            return; // мягко завершаю задачу
                            // token.ThrowIfCancellationRequested(); // генерируем исключение  
                        }
                        
                        await SendProcessNotification(playerName: player.Name, nGame: n, nMove: i, token);
                        await AnalyzePosition(position);

                        if (position.IsEvaluated())
                        {
                            if (PositionEvaluation.IsMistake(colorInGame, prevPosition.PositionEvaluation!.Cp, position.PositionEvaluation!.Cp, mistakePrecision))
                                prevPosition.SetPositionIsMistake(true);
                        }
                        else
                        {
                            _logger.LogError("Ошибка получения оценки для позиции: \"{position.Id}\"", position.Id);
                             break;
                        }

                        prevPosition = position; // текущую делаю предыдущей
                    }
                }
            }
        }
    }
}