using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class ReportBuildingService
    {
        private readonly GoogleDocumentService _documentService;

        public ReportBuildingService(GoogleDocumentService documentService)
        {
            _documentService = documentService;
        }

        public async Task<HFIAgentReport> BuildHouseholdFinancialIntelligenceReport()
        {
            HFIAgentReport report = new HFIAgentReport { AgentDashboard = new AgentDashboard() };

            var rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.TOTAL_METRICS);
            report.AgentDashboard.TotalMonthlyBills = AddDashboardMetric(rows[0]);
            report.AgentDashboard.TotalCreditCardBalance = AddDashboardMetric(rows[1]);
            report.AgentDashboard.TotalCreditCardLimit = AddDashboardMetric(rows[2]);
            report.AgentDashboard.OverallCreditUtilization = AddDashboardMetric(rows[3]);
            report.AgentDashboard.HighestUtilizationCard = AddDashboardMetric(rows[4]);
            report.AgentDashboard.HightestUtilization = AddDashboardMetric(rows[5]);
            report.AgentDashboard.TotalRegularLoanBalance = AddDashboardMetric(rows[6]);
            report.AgentDashboard.TotalShortTermBalance = AddDashboardMetric(rows[7]);
            report.AgentDashboard.MaxExtraAllocationRule = AddDashboardMetric(rows[8]);
            report.AgentDashboard.OpenDataQualityIssues = AddDashboardMetric(rows[9]);

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.OVERALL_METRICS);

            report.AgentDashboard.CreditCards = AddOverallMetric(rows[0]);
            report.AgentDashboard.RegularLoans = AddOverallMetric(rows[1]);
            report.AgentDashboard.ShortTermFinancing = AddOverallMetric(rows[0]);


            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.AGENT_PRIORITIES);
            foreach (var row in rows)
            {
                report.AgentDashboard.AgentRunPriorities.Add(new AgentRunPriority
                {
                    Priority = row[0].ToString(),
                    Name = row[1].ToString(),
                    Description = row[2].ToString()
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.INCOMES);
            foreach (var row in rows)
            {
                report.Incomes.Add(new Income
                {
                    Person = row.SafeString(0),
                    PayFrequency = row.SafeString(1),
                    GrossPay = row.SafeString(2),
                    NetPay = row.SafeString(3),
                    NextPayDate = row.SafeString(4),
                    Bonus = row.SafeString(5),
                    Source = row.SafeString(6),
                    Notes = row.SafeString(7),
                    DataConfidence = row.SafeString(8),
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.MONTHLY_BILLS);
            foreach (var row in rows)
            {
                report.MonthlyBills.Add(new MonthlyBills
                {
                    BillName = row.SafeString(0),
                    DueDate = row.SafeString(1),
                    Amount = row.SafeString(2),
                    PayWindow = row.SafeString(3),
                    Category = row.SafeString(4),
                    PaymentAccount = row.SafeString(5),
                    Weight = row.SafeString(6),
                    Required = row.SafeString(7),
                    Source = row.SafeString(8),
                    Notes = row.SafeString(9)
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.CREDIT_CARDS);
            foreach (var row in rows)
            {
                report.CreditCards.Add(new CreditCard
                {
                    CardName = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    MinimumPayment = row.SafeString(2),
                    Adjustment = row.SafeString(3),
                    RealAmount = row.SafeString(4),
                    Balance = row.SafeString(5),
                    Limit = row.SafeString(6),
                    Untilization = row.SafeString(7),
                    PaymentPct = row.SafeString(8),
                    PayoffTarget = row.SafeString(9),
                    StatementCloseDate = row.SafeString(10),
                    APR = row.SafeString(11),
                    Owner = row.SafeString(12),
                    Source = row.SafeString(13),
                    Notes = row.SafeString(14)
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.LOANS);
            foreach (var row in rows)
            {
                report.Loans.Add(new Loan
                {
                    LoanName = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    OriginalPayment = row.SafeString(2),
                    Adjustment = row.SafeString(3),
                    MonthlyPayment = row.SafeString(4),
                    Balance = row.SafeString(5),
                    StarteDate = row.SafeString(6),
                    EndDate = row.SafeString(7),
                    Total = row.SafeString(8),
                    PIF = row.SafeString(9),
                    APR = row.SafeString(10),
                    Owner = row.SafeString(12),
                    Priority = row.SafeString(13),
                    Source = row.SafeString(14),
                    Notes = row.SafeString(15)
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.SHORT_TERM_FINANCING);
            foreach (var row in rows)
            {
                report.ShortTermFinancing.Add(new ShortTermFinance
                {
                    Name = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    MonthlyPayment = row.SafeString(4),
                    Balance = row.SafeString(5),
                    PayOff = row.SafeString(6),
                    RateAPR = row.SafeString(7),
                    StarteDate = row.SafeString(8),
                    EndDate = row.SafeString(9),
                    Total = row.SafeString(10),
                    Source = row.SafeString(11),
                    Notes = row.SafeString(12)
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.PAYCHECK_CASH_FLOW);
            foreach (var row in rows)
            {
                report.PaychecCashFlow.Add(new CashFlow
                {
                    PayDate = row.SafeString(0),
                    NetIncome = row.SafeString(1),
                    Bonus = row.SafeString(4),
                    CUTX = row.SafeString(5),
                    WorkExpenses = row.SafeString(6),
                    ChasePyN4 = row.SafeString(7),
                    Mortgage = row.SafeString(8),
                    Groceries = row.SafeString(9),
                    Other = row.SafeString(10),
                    Bills = row.SafeString(11),
                    CC = row.SafeString(12),
                    RegularLoans = row.SafeString(13),
                    ShortTerm = row.SafeString(14),
                    CalculatedTotalExpense = row.SafeString(15),
                    AfterExpenses = row.SafeString(16),
                    Source = row.SafeString(17),
                    Notes = row.SafeString(18),
                    TwentiyFivePercentBuffer = row.SafeString(19),
                    MaxUsableExtraSeventyFivePercent = row.SafeString(20)
                });
            }

            rows = await _documentService.ReadRangeAsync(HFIAgentReport.ReportCells.SHEET_ID, HFIAgentReport.ReportCells.MORTGAGE_REFI_READINESS);
            foreach (var row in rows)
            {
                report.MortgageRefiReadinesses.Add(new MortgageRefiReadiness
                {
                    Input = row.SafeString(0),
                    Value = row.SafeString(1),
                    Unit = row.SafeString(2),
                    Source = row.SafeString(3),
                    Status = row.SafeString(4),
                    Notes = row.SafeString(5),
                    AgentUse = row.SafeString(6),
                    LastUpdated = row.SafeString(7)
                });
            }

            return report;
        }

        private DashboardMetric AddDashboardMetric(IList<object> row)
        {
            return new DashboardMetric
            {
                Name = row[0].ToString(),
                Value = row[1].ToString(),
                Notes = row[2].ToString()
            };
        }

        private OverallMetrics AddOverallMetric(IList<object> row)
        {
            return new OverallMetrics
            {
                DebtType = row[0].ToString(),
                Balance = row[1].ToString(),
                PercentageOfTotal = row[2].ToString()
            };
        }
    }
}
