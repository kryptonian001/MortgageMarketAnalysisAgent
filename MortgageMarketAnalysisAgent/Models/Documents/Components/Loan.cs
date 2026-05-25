namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class Loan
    {
        public string LoanName { get; set; }
        public string DueDay { get; set; }
        public string OriginalPayment { get; set; }
        public string Adjustment { get; set; }
        public string MonthlyPayment { get; set; }
        public string Balance { get; set; }
        public string StarteDate { get; set; }
        public string EndDate { get; set; }
        public string Total { get; set; }
        public string PIF { get; set; }
        public string APR { get; set; }
        public string Owner { get; set; }
        public string Priority { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }
    }
}
