﻿using System.Text;
using System.Text.RegularExpressions;

namespace Domain;

public static class ChessHelper
{
    /// <summary>
    /// Позиция по умолчанию в формате fwn
    /// </summary>
    public const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    /// <summary>
    /// Регулярное выражения проверки корректности формату fen
    /// </summary>     
    private static readonly Regex RegexFen = new(@"\s*^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+)\s([b|w])\s(-|[K|Q|k|q]{1,4})\s(-|[a-h][1-8])\s(\d+\s\d+)$", RegexOptions.Compiled);
    // private static readonly Regex RegexFen = new(@"^(?:[pnbrqkPNBRQK1-8]+/){7}[pnbrqkPNBRQK1-8]+\s(?:b|w)\s(?:-|K?Q?k?q)\s(?:-|[a-h][3-6])(?:\s+(?:\d+)){0,2}$", RegexOptions.Compiled); // почему-то иногда сбоит
    // private static readonly Regex RegexFen = new(@"\s*^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+)\s([b|w])\s([K|Q|k|q]{1,4})\s(-|[a-h][1-8])\s(\d+\s\d+)$", RegexOptions.Compiled); // тоже через раз

    /// <summary>
    /// Фигуры используемые для нотации
    /// </summary>
    private static char[] shapes = { 'k', 'q', 'n', 'b', 'r', 'p' };

    /// <summary>
    /// Проверяет корректно ли представление позиции в fen формате 
    /// </summary>
    /// <param name="fen"></param>
    /// <returns></returns>
    public static bool CheckValid(string fen) => RegexFen.IsMatch(fen);

    /// <summary>
    /// Извлекает данные из шапки отдельного pgn 
    /// </summary>
    /// <param name="Headers"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string TryExtractHeaderValue(IReadOnlyDictionary<string, string> Headers, string key)
        => Headers.ContainsKey(key) ? Headers[key] : string.Empty;

    /// <summary>
    /// Разделить список игр PGN на последовательность частей
    /// </summary>
    /// <param name="stringAllPGN"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetSplittedPGNmass(string? stringAllPGN) => stringAllPGN?.Split("\n\n\n") ?? Enumerable.Empty<string>();

    /// <summary>
    /// Получить нотацию вида Chessbase на основе fen
    /// </summary>
    /// <param name="fen"></param>
    /// <returns></returns>
    public static string FenToChessbaseNotation(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")  // создает строку позиции для chessbase 
    {
        StringBuilder pos = new();
        Dictionary<string, string> wb = new();

        if (Chess.ChessBoard.TryLoadFromFen(fen, out var board) && board is not null)
        {
            for (char i = 'a'; i < 'i'; i++)
            {
                for (short j = 1; j <= 8; j++)
                {
                    if (board[i, j] is not null)
                    {
                        var key = $"{board[i, j]!.Color.AsChar}{board[i, j]!.Type.AsChar}";

                        if (wb.ContainsKey(key))
                            wb[key] += $",{i}{j}";
                        else
                            wb.Add(key, $"{i}{j}");
                    }
                }
            }

            CreatePosByColor('w');
            pos.Append('/');
            CreatePosByColor('b');
        }

        return pos.ToString();

        void CreatePosByColor(char color)
        {
            pos.Append($"{color}K{wb.First(x => x.Key.Equals($"{color}k")).Value}"); // короли есть всегда!

            foreach (char shape in shapes)
                pos.Append($"{GetFigure($"{color}{shape}")}");            
        }

        string GetFigure(string find)
            => wb.TryGetValue(find, out string? queen) ? $",{find.ToUpper().Last()}{queen}" : string.Empty;
    }
}