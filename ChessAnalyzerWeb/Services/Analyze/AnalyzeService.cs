using ChessAnalyzerApi.Extensions;
using ChessAnalyzerApi.Hubs;
using Domain.GameAggregate;
using Microsoft.AspNetCore.SignalR;
using Position = Domain.GameAggregate.Position;

namespace ChessAnalyzerApi.Services.Analyze;

public class AnalyzerService : IAnalyzeService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    private bool isRunning = false;
    private readonly IEnumerable<IPositionEvaluation> _EvaluationServices;

    public AnalyzerService(IEnumerable<IPositionEvaluation> EvaluationServices, IHubContext<NotificationHub> hubContext) //ILogger<AnalyzerService> logger
    {
        _EvaluationServices = EvaluationServices ?? Enumerable.Empty<IPositionEvaluation>();
        _hubContext = hubContext;
    }

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

    public bool HaveAnyEvaluationServises() => _EvaluationServices.Any();

    public async Task RunAnalyzePlayerGames(Player player, double mistakePrecision, CancellationToken token = default)
    {
        if (HaveAnyEvaluationServises())
        {
            foreach (var (game, n) in player.Games.WithIndex())
            {
                if (game.HaveAnyPositions()) // позиции есть (отпарсили игру успешно)
                {
                    var colorInGame = player.GetPlayerColorInGame(game); // цвет player в этой игре
                    Position prevPosition = game.Positions.First();
                    prevPosition.SetUserEvaluation(0.0d); // у первой оценка 0

                    foreach (var (position, i) in game.Positions.Skip(1).WithIndex()) // для всех позиций начиная со второй (пропуская первую) получаю оценку
                    {
                        if (token.IsCancellationRequested)  // проверяем наличие сигнала отмены задачи
                        {
                            token.ThrowIfCancellationRequested(); // генерируем исключение  
                        }

                        await _hubContext.Clients.All.SendAsync(method: "Notification", 
                            $"Игра номер {n + 1}. Анализирую ход {Math.Ceiling((double)i / 2)}",
                            cancellationToken: token); // вебсокет

                        await AnalyzePosition(position);
                        if (PositionEvaluation.IsMistake(colorInGame, prevPosition.PositionEvaluation.Cp, position.PositionEvaluation.Cp, mistakePrecision))
                            player.AddToMistakes(prevPosition);

                        prevPosition = position; // текущую делаю предыдущей
                    }
                }
                else
                {
                    // nonParsed.Add(onePGN);// те игры которые не получилось отпарсить (!!!!!!!!!!!!!!!!!!!!!!! ПЕРЕПИСАТЬ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!)
                }
            }
        }
    }
}