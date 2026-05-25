using MortgageMarketAnalysisAgent.Models.Documents.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Documents
{
    /// <summary>
    /// Household Financial Intelligence Agent report
    /// </summary>
    public class HouseholdFinancialIntelligenceModel
    {
        public static class ReportCells
        {
            public static readonly string SHEET_ID = "1Fcf4EDTGNmfG1Ku_MmlahCUlkSkyeddWx9UVHSP6eLs";
            public static readonly string TOTAL_METRICS = "Agent Dashboard!A4:C13";
            public static readonly string OVERALL_METRICS = "Agent Dashboard!E4:G6";
            public static readonly string AGENT_PRIORITIES = "Agent Dashboard!A16:C20";
            public static readonly string INCOMES = "Income!A2:I10";
            public static readonly string MONTHLY_BILLS = "Monthly Bills!A2:J50";
            public static readonly string CREDIT_CARDS  = "Credit Cards!A2:O20";
            public static readonly string LOANS = "Loans!A2:O10";
            public static readonly string SHORT_TERM_FINANCING = "Short-Term Financing!A2:K20";
            public static readonly string PAYCHECK_CASH_FLOW = "Paycheck Cash Flow!A2:S52";
            public static readonly string MORTGAGE_REFI_READINESS = "Mortgage Refi Readiness!A2:H11";
        }

        public AgentDashboard AgentDashboard { get; set; }
        public List<Income> Incomes { get; set; } = new();
        public List<MonthlyBills> MonthlyBills { get; set; } = new();
        public List<CreditCard> CreditCards { get; set; } = new();
        public List<Loan> Loans { get; set; } = new();
        public List<ShortTermFinance> ShortTermFinancing { get; set; } = new();
        public List<CashFlow> PaychecCashFlow { get; set; } = new();
        public List<MortgageRefiReadiness> MortgageRefiReadinesses { get; set; } = new();
    }


}
