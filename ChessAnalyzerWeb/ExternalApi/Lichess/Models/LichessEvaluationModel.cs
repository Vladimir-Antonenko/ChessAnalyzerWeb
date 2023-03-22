using System.Collections.Generic;

namespace ChessAnalyzerApi.ExternalApi.Lichess.Models;

public class LichessEvaluationModel
{
    public string fen { get; set; }
    public int knodes { get; set; }
    public int depth { get; set; }
    public List<PvModel> pvs { get; set; }
}