using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class MonthlyBills
    {
        public string BillName { get; set; }
        public string DueDate { get; set; }
        public string Amount { get; set; }
        public string PayWindow { get; set; }
        public string Category { get; set; }
        public string PaymentAccount { get; set; }
        public string Weight { get; set; }
        public string Required { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }
    }
}
