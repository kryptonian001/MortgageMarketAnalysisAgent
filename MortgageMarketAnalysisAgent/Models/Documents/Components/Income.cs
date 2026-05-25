using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class Income
    {
        public string Person { get; set; }
        public string PayFrequency{ get; set; }
        public string GrossPay { get; set; }
        public string NetPay { get; set; }
        public string NextPayDate { get; set; }
        public string Bonus { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }
        public string DataConfidence { get; set; }
    }
}
