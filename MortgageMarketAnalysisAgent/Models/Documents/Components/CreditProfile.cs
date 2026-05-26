namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class CreditProfile
    {
        public string Person { get; set; }
        public string Fico8 { get; set; }
        public string Fico5 { get; set; }
        public string Fico4 { get; set; }
        public string Fico2 { get; set; }
        public string Vantage3_0 { get; set; }
        public string ScoreDate { get; set; }
        public int? MortgageMiddleScore { get; set; }
        public string Notes { get; set; }
        public string DataConfidence { get; set; }
    }
}
