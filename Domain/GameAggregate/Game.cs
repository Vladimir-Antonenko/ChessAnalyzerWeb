namespace Domain.GameAggregate;

public class Game
{
    public int Id { get; private set; }
    public string Pgn { get; private set; } // pgn строка игры
    public string WhiteGamer { get; private set; } // белые
    public string BlackGamer { get; private set; } // черные
    public string Result { get; private set; } // строка результата
    public DateTime DateGame { get; private set; } // когда играли

    private readonly List<Position> positions = new();
    public IReadOnlyCollection<Position> Positions => positions; // разные позиции в игре

    private Game() { } // для ef core

    private Game(string gamePgn)
    {
        Pgn = gamePgn;
        ExtractPositionsFromPgn();
    }

    private void ExtractPositionsFromPgn() // извлекаю позиции из pgn
    {
        if (Chess.ChessBoard.TryLoadFromPgn(Pgn, out var game) && game?.MoveIndex > -1) // если удалось отпарсить одну pgn в gameBoard
        {
            WhiteGamer = ChessHelper.TryExtractHeaderValue(game.Headers, key: "White");
            BlackGamer = ChessHelper.TryExtractHeaderValue(game.Headers, key: "Black");
            Result = ChessHelper.TryExtractHeaderValue(game.Headers, key: "Result");
            DateGame = DateTime.Parse(ChessHelper.TryExtractHeaderValue(game.Headers, key: "Date"));

            for (int i = 0; i < game.MovesToSan.Count; i++) // MovesToSan - ходы белых+черных
            {
                game.MoveIndex = i - 1; // перехожу к следующемеу ходу

                var fen = game.ToFen();
                var color = i % 2 == 0 ? Color.White : Color.Black;
                var executedMove = game.MovesToSan[i];

                var position = Position.Create(fen, color, executedMove);
                AddPosition(position);
            }
        }
    }

    public void AddPosition(Position position) => positions.Add(position);
   
    public bool HaveAnyPositions() => positions.Any();

    public static Game Create(string pgn) => new(pgn);
}