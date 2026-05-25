namespace MortgageMarketAnalysisAgent.Models.Documents.Components
{
    public class AgentDashboard
    {
        public DashboardMetric TotalMonthlyBills { get; set; }
        public DashboardMetric TotalCreditCardBalance { get; set; }
        public DashboardMetric TotalCreditCardLimit { get; set; }
        public DashboardMetric OverallCreditUtilization { get; set; }
        public DashboardMetric HighestUtilizationCard { get; set; }
        public DashboardMetric HightestUtilization { get; set; }
        public DashboardMetric TotalRegularLoanBalance { get; set; }
        public DashboardMetric TotalShortTermBalance { get; set; }
        public DashboardMetric MaxExtraAllocationRule { get; set; }
        public DashboardMetric OpenDataQualityIssues { get; set; }

        public OverallMetrics CreditCards { get; set; }
        public OverallMetrics RegularLoans { get; set; }
        public OverallMetrics ShortTermFinancing { get; set; }

        public List<AgentRunPriority> AgentRunPriorities { get; set; } = new List<AgentRunPriority>();
    }
}
