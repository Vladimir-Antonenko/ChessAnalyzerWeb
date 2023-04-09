using System.Text.RegularExpressions;

namespace Domain;

/// <summary>
/// Вспомогательные регулярные выражения для шахмат
/// </summary>
internal class Regexes
{
    internal const string SanOneMovePattern = @"(^([PNBRQK])?([a-h])?([1-8])?(x|X|-)?([a-h][1-8])(=[NBRQ]| ?e\.p\.)?|^O-O(-O)?)(\+|\#|\$)?$";

    internal const string SanMovesPattern = @"([PNBRQK]?[a-h]?[1-8]?[xX-]?[a-h][1-8](=[NBRQ]| ?e\.p\.)?|O-O(?:-O)?)[+#$]?";

    internal const string HeadersPattern = @"\[(.*?) ""(.*?)""\]";

    internal const string AlternativesPattern = @"\(.*?\)";

    internal const string CommentsPattern = @"\{.*?\}";

    internal const string FenPattern = @"^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+) ([b|w]) (-|[K|Q|k|q]{1,4}) (-|[a-h][36]) (\d+ \d+)$";

    internal const string PiecePattern = "^[wb][bknpqr]$";

    internal const string FenPiecePattern = "^([bknpqr]|[BKNPQR])$";

    internal const string PositionPattern = "^[a-h][1-8]$";

    internal const string MovePattern = @"^{(([wb][bknpqr]) - )?([a-h][1-8]) - ([a-h][1-8])( - ([wb][bknpqr]))?( - (o-o|o-o-o|e\.p\.|=|=q|=r|=b|=n))?( - ([+#$]))?}$";

    internal readonly static Regex SanOneMove = new(SanOneMovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex SanMoves = new(SanMovesPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Headers = new(HeadersPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Alternatives = new(AlternativesPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Comments = new(CommentsPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Fen = new(FenPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Piece = new(PiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex FenPiece = new(FenPiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Position = new(PositionPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex Move = new(MovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
}