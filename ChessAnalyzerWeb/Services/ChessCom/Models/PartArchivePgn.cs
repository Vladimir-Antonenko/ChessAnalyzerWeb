namespace ChessAnalyzerApi.Services.ChessCom.Models;

public class PartArchivePgn
{
    public string PartContent { get; set; }

    public override string ToString() => PartContent;
}