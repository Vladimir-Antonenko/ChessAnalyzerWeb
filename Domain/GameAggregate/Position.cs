namespace Domain.GameAggregate;

public class Position
{
    public int Id { get; private set; }
    public string Fen { get; private set; } // позиция
    public string YourMove { get; private set; } = ""; // сделанный ход
    public Color WhoseMove { get; private set; } // чей ход
    public PositionEvaluation? PositionEvaluation { get; private set; } // оценка
    public int? GameId { get; private set; } // в какой игре встретилась
    public Game Game { get; private set; }
    public bool IsMistake { get; private set; } // в позиции совершена ошибка

    /// <summary>
    /// Пустой конструктор для EF Core
    /// </summary>
    private Position() { }

    /// <summary>
    /// Конструктор для позиции
    /// </summary>
    /// <param name="posFen">Позиция в формате fen строки</param>
    /// <param name="posWhoseMove">Цвет чей ход</param>
    /// <param name="posYourMove">Сделанный игроком ход в позиции</param>
    /// <exception cref="InvalidDataException"></exception>
    private Position(string posFen, Color posWhoseMove, string posYourMove)
    {
        if (IsCorrectFen(posFen))
        {
            Fen = posFen;
            WhoseMove = posWhoseMove;
            YourMove = posYourMove;
        }
        else
            throw new InvalidDataException($"Позиция {posFen}\n некорректна!");       
    }

    /// <summary>
    /// Установить параметр "ошибка" для позиции
    /// </summary>
    /// <param name="value">Значение определяющее ошибка это или нет</param>
    /// <exception cref="InvalidDataException"></exception>
    public void SetPositionIsMistake(bool value)
    {
        if (!IsEvaluated())
            throw new InvalidDataException("Позиция без оценки не может считаться ошибкой");
        else
            IsMistake = value;
    }

    /// <summary>
    /// Установить оценку по переданным параметрам извне
    /// </summary>
    /// <param name="cp">Оценка</param>
    /// <param name="depth">Глубина расчета</param>
    /// <param name="move">Лучший ход</param>
    public void SetEvaluation(double cp, int depth = 0, string move = "None")
    {
        if (!IsEvaluated())
            PositionEvaluation = PositionEvaluation.Create(Fen, cp, move);
        else
            PositionEvaluation!.ChangeEvaluation(cp, depth, move);
    }

    /// <summary>
    /// Установить оценку на основе переданной оценки извне
    /// </summary>
    /// <param name="posEval">Оценка позиции</param>
    public void SetEvaluation(PositionEvaluation posEval)
    {
        if (!IsEvaluated())
            PositionEvaluation = posEval;
        else
            PositionEvaluation!.ChangeEvaluation(posEval!.Cp, posEval!.Depth, posEval!.OneMove);
    }

    /// <summary>
    /// Получить оценку позиции из внешнего ресурса
    /// </summary>
    /// <param name="serviceEvaluation">Сервис оценки позииции</param>
    /// <returns></returns>
    public async Task GetPositionEvaluation(IPositionEvaluation serviceEvaluation)
    {
        var positionEvaluation = await serviceEvaluation.GetPositionEvaluationAsync(Fen);
        SetEvaluation(positionEvaluation);
    }

    /// <summary>
    /// Проверка корректности позиции в виде Fen нотации
    /// </summary>
    /// <param name="fen"></param>
    /// <returns></returns>
    public static bool IsCorrectFen(string fen) => ChessHelper.CheckValid(fen);

    /// <summary>
    /// Проверка оценена ли позиция
    /// </summary>
    /// <returns></returns>
    public bool IsEvaluated() => !PositionEvaluation?.IsEmpty() ?? false;

    /// <summary>
    /// Фабричный метод для создания новой позиции
    /// </summary>
    /// <param name="fen">Позиция в формате fen строки</param>
    /// <param name="whoseMove">Цвет чей ход</param>
    /// <param name="yourMove">Сделанный игроком ход в позиции</param>
    /// <returns></returns>
    public static Position Create(string fen, Color whoseMove, string yourMove = "") => new(fen, whoseMove, yourMove);

    // public string GetSvgBase64(IFenSvgGenerator generator) => generator.GetSvgInBase64();
}