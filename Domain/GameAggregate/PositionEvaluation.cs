namespace Domain.GameAggregate;

public class PositionEvaluation
{
    public int Id { get; private set; }
    public int Depth { get; private set; }
    public double Cp { get; private set; }
    public string OneMove { get; private set; } = "";
    public string Fen { get; private set; } = "";

    private PositionEvaluation() { }

    private PositionEvaluation(string fen, double cp, string oneMove, int depth)
    {
        this.Fen = fen;
        this.Cp = cp;
        this.OneMove = oneMove;
        this.Depth = depth;
    }

    public bool IsEmpty() => OneMove == string.Empty;

    public static bool IsMistake(Color colorInGame, double prevCp, double curCp, double mistakePrecision)
    {
        // |N-P| >= m && sign* (N-P)<0 (был перепад более чем на заданную величину) и является ли это ошибкой для данного цвета (мб улучшение позиции для данного цвета)
        var sign = colorInGame.Equals(Color.White) ? 1 : -1;
        return Math.Abs(curCp - prevCp) >= mistakePrecision && sign * (curCp - prevCp) < 0.0d;
    }

    public static PositionEvaluation Create(string fen, double cp = 0.0d, string oneMove = "", int depth = 0) 
        => new(fen, cp, oneMove, depth); 
}