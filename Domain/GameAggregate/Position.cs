using Domain.ChessSvgGenerator;

namespace Domain.GameAggregate;

public class Position
{
    public int Id { get; private set; }
    public string Fen { get; private set; } // позиция
    public string YourMove { get; private set; } = ""; // сделанный ход
    public Color WhoseMove { get; private set; } // чей ход
    public PositionEvaluation PositionEvaluation { get; private set; } // оценка
    public int? GameId { get; private set; } // в какой игре встретилась
    public Game Game { get; private set; }
    public bool IsMistake { get; private set; }

    private Position(string fen, Color whoseMove, string yourMove)
    {
        if (IsCorrectFen(fen))
        {
            Fen = fen;
            WhoseMove = whoseMove;
            YourMove = yourMove;
            this.PositionEvaluation = PositionEvaluation.Create(fen);
        }
        else
            throw new InvalidDataException($"Позиция {fen}\n некорректна!");       
    }

    public void SetPositionIsMistake(bool value) => IsMistake = value;
    public void SetUserEvaluation(double cp, string move = "None") => PositionEvaluation = PositionEvaluation.Create(Fen, cp, move);

    public async Task GetPositionEvaluation(IPositionEvaluation serviceEvaluation) // получить оценку позиции
    {
        PositionEvaluation = await serviceEvaluation.GetPositionEvaluationAsync(Fen);
    }

    public string GetSvgBase64(IFenSvgGenerator generator) => generator.GetSvgInBase64();

    public bool IsCorrectFen(string fen) => ChessHelper.CheckValid(fen);
    public bool IsEvaluated() => !PositionEvaluation.IsEmpty(); // позиция оценена?

    public static Position Create(string fen, Color whoseMove, string yourMove = "") => new(fen, whoseMove, yourMove);
}