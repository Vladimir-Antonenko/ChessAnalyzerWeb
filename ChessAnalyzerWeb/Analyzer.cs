using ChessAnalyzerApi.Extensions;
using Domain.GameAggregate;
using Position = Domain.GameAggregate.Position;

namespace ChessAnalyzerApi
{
    public class Analyzer
    {
        private readonly List<IPositionEvaluation> _evaluationService = new();
        private readonly Player _player;
        private readonly double _mistakePrecision;
        private readonly CancellationToken token;
        private bool isRunning = false;

        private Analyzer(Player player, double mistakePrecision, CancellationToken token)
        {
            _player = player;
            _mistakePrecision = mistakePrecision;
            this.token = token;
        }

        public bool IsRunning() => isRunning;

        public bool HaveAnyEvaluationServises() => _evaluationService.Any();

        public void AddAnalyzeService(IPositionEvaluation analysisService) => _evaluationService.Add(analysisService);

        public async Task RunAnalyzePlayerGames()
        {
            isRunning = true;

            if (HaveAnyEvaluationServises())
            {
                foreach (var (game, n) in _player.Games.WithIndex())
                {
                    if (game.HaveAnyPositions()) // позиции есть (отпарсили игру успешно)
                    {
                        var colorInGame = _player.GetPlayerColorInGame(game); // цвет player в этой игре
                        Position prevPosition = game.Positions.First();
                        prevPosition.SetUserEvaluation(0.0d); // у первой оценка 0

                        foreach (var (position, i) in game.Positions.Skip(1).WithIndex()) // для всех позиций начиная со второй (пропуская первую) получаю оценку
                        {
                            if (token.IsCancellationRequested)  // проверяем наличие сигнала отмены задачи
                            {
                                token.ThrowIfCancellationRequested(); // генерируем исключение  
                            }

                            // ТУТ вызвать метод отправки через вебСокет!!
                            //  InfoLoad = $"Игра номер {n + 1}. Анализирую ход {Math.Ceiling((double)i / 2)}"; // тут подумать как извещать о ходе выполнения!! скорее всегоо в событии подписаться на засланную переменную которую просто плюсовать в классе

                            await AnalyzePosition(position);

                            if (PositionEvaluation.IsMistake(colorInGame, prevPosition.PositionEvaluation.Cp, position.PositionEvaluation.Cp, _mistakePrecision))
                                _player.AddToMistakes(prevPosition);

                            prevPosition = position; // текущую делаю предыдущей
                        }
                    }
                    else
                    {
                        // nonParsed.Add(onePGN);// те игры которые не получилось отпарсить (!!!!!!!!!!!!!!!!!!!!!!! ПЕРЕПИСАТЬ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!)
                    }
                }
            }

            isRunning = false;
        }

        private async Task AnalyzePosition(Position position)
        {
            foreach (var evaluationService in _evaluationService)
            {
                if (!position.IsEvaluated()) // оценка не найдена?
                {
                    await position.GetPositionEvaluation(evaluationService);
                }
            }
        }

        public static Analyzer Create(Player player, double mistakePrecision, CancellationToken token) => new(player, mistakePrecision, token);
    }
}