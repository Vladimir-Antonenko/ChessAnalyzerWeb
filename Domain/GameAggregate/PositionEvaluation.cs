namespace Domain.GameAggregate;

public class PositionEvaluation
{
    public int Id { get; private set; }
    public int Depth { get; private set; }
    public double Cp { get; private set; }
    public string OneMove { get; private set; } = "";
    public string Fen { get; private set; } = "";

    /// <summary>
    /// Для Ef Core
    /// </summary>
    private PositionEvaluation() { }

    /// <summary>
    /// Конструктор для оценки позиции
    /// </summary>
    /// <param name="fen">Позиция в формате fen строки</param>
    /// <param name="cp">Оценка</param>
    /// <param name="oneMove">Лучший ход</param>
    /// <param name="depth">Глубина расчета</param>
    private PositionEvaluation(string fen, double cp, string oneMove, int depth)
    {
        this.Fen = fen;
        this.Cp = cp;
        this.OneMove = oneMove;
        this.Depth = depth;
    }

    /// <summary>
    /// Найдена ли оценка для данной позиции
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => OneMove == string.Empty;

    /// <summary>
    /// Изменить оценку позиции
    /// </summary>
    /// <param name="cp">Оценка</param>
    /// <param name="depth">Глубина расчета</param>
    /// <param name="oneMove">Лучший ход</param>
    public void ChangeEvaluation(double cp, int depth, string oneMove)
    {
        this.Cp = cp;
        this.Depth = depth;
        this.OneMove = oneMove;
    }

    /// <summary>
    /// Является ли данная 
    /// </summary>
    /// <param name="colorInGame">Цвет игрока в партии</param>
    /// <param name="prevCp">Предыдущая оценка</param>
    /// <param name="curCp">Текущая оценка</param>
    /// <param name="mistakePrecision">Точность перепада оценки</param>
    /// <returns></returns>
    public static bool IsMistake(Color colorInGame, double prevCp, double curCp, double mistakePrecision)
    {
        // |N-P| >= m && sign* (N-P)<0 (был перепад более чем на заданную величину) и является ли это ошибкой для данного цвета (мб улучшение позиции для данного цвета)
        var sign = colorInGame.Equals(Color.White) ? 1 : -1;
        return Math.Abs(curCp - prevCp) >= mistakePrecision && sign * (curCp - prevCp) < 0.0d;
    }

    /// <summary>
    /// Фабричный метод создания новой оценки позиции
    /// </summary>
    /// <param name="fen">Позиция в формате fen строки</param>
    /// <param name="cp">Оценка</param>
    /// <param name="oneMove">Лучший ход</param>
    /// <param name="depth">Глубина расчета</param>
    /// <returns></returns>
    public static PositionEvaluation Create(string fen, double cp = 0.0d, string oneMove = "", int depth = 0) 
        => new(fen, cp, oneMove, depth); 
}