namespace ChessAnalyzerApi.Services.Stockfish.NET.Models;

public class StockfishParameters
{
    #region Параметры
    // init; c# 9.0 доступно для изменения только в качестве инициализации один раз, далее только для чтения
    public string pathEngine { get; init; }// = @"D:\Chess\stockfish_20090216_x64.exe"; 
    public int Depth { get; init; }// = 21;  
    public int Threads { get; init; } 
    public Settings st { get; init; }
    #endregion

    public StockfishParameters(int depth, int threads = 12)
    {
        pathEngine = new DirectoryInfo(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"Services\Stockfish\Engine\stockfish_15_x64_avx2.exe"))).ToString(); //@"Stockfish.Engine\stockfish_20090216_x64.exe"
        Depth = depth;

        st = new Settings
        {
            Contempt = 0,
            Threads = threads,
            SkillLevel = 20,
            SlowMover = 10
        };
    }
}
