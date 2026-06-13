namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class CreditCard
    {
        public string CardName { get; set; }
        public string DueDay { get; set; }
        public string MinimumPayment { get; set; }
        //public string Adjustment { get; set; }
        //public string RealAmount { get; set; }
        public string Balance { get; set; }
        public string Limit { get; set; }
        public string Untilization { get; set; }
        public string PaymentPct { get; set; }
        public string PayoffTarget { get; set; }
        public string StatementCloseDate { get; set; }
        public string APR { get; set; }
        public string Owner { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }
    }
}
