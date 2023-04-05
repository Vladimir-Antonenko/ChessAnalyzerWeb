namespace ChessAnalyzerApi.Services.ChessDB.Models
{
    public class QueryPvModel
    {
        public string status { get; set; }
        public int score { get; set; }
        public int depth { get; set; }
        public List<string> pv { get; set; }
        public List<string> pvSAN { get; set; }
    }
}