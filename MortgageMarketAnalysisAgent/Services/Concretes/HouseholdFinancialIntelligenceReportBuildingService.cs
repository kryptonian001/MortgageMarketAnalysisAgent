using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class HouseholdFinancialIntelligenceReportBuildingService : IReportBuildingService
    {
        private readonly GoogleDocumentService _documentService;

        public HouseholdFinancialIntelligenceReportBuildingService(GoogleDocumentService documentService)
        {
            _documentService = documentService;
        }

        public async Task<HouseholdFinancialIntelligenceModel> BuildHouseholdFinancialIntelligenceReport()
        {
            HouseholdFinancialIntelligenceModel report = new HouseholdFinancialIntelligenceModel { AgentDashboard = new AgentDashboard() };

            var rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.TOTAL_METRICS);
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

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.OVERALL_METRICS);

            report.AgentDashboard.CreditCards = AddOverallMetric(rows[0]);
            report.AgentDashboard.RegularLoans = AddOverallMetric(rows[1]);
            report.AgentDashboard.ShortTermFinancing = AddOverallMetric(rows[0]);


            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.AGENT_PRIORITIES);
            foreach (var row in rows)
            {
                report.AgentDashboard.AgentRunPriorities.Add(new AgentRunPriority
                {
                    Priority = row[0].ToString(),
                    Name = row[1].ToString(),
                    Description = row[2].ToString()
                });
            }

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.INCOMES);
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

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.MONTHLY_BILLS);
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

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_CARDS);
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

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_PROFILES);
            foreach (var row in rows)
            {
                var profile = new CreditProfile
                {
                    Person = row.SafeString(0),
                    Fico8 = row.SafeString(1),
                    Fico5 = row.SafeString(2),
                    Fico4 = row.SafeString(3),
                    Fico2 = row.SafeString(4),
                    Vantage3_0 = row.SafeString(5),
                    ScoreDate = row.SafeString(6),
                    Notes = row.SafeString(7),
                    DataConfidence = row.SafeString(8)
                };

                profile.MortgageMiddleScore = profile.CalculateMortgageMiddleScore();

                report.CreditProfiles.Add(profile);
            }

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.LOANS);
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

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.SHORT_TERM_FINANCING);
            foreach (var row in rows)
            {
                report.ShortTermFinancing.Add(new ShortTermFinance
                {
                    Name = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    MonthlyPayment = row.SafeString(2),
                    Balance = row.SafeString(3),
                    PayOff = row.SafeString(4),
                    RateAPR = row.SafeString(5),
                    StarteDate = row.SafeString(6),
                    EndDate = row.SafeString(7),
                    Total = row.SafeString(8),
                    Source = row.SafeString(9),
                    Notes = row.SafeString(10)
                });
            }

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.PAYCHECK_CASH_FLOW);
            foreach (var row in rows)
            {
                report.PaychecCashFlow.Add(new CashFlow
                {
                    PayDate = row.SafeString(0),
                    NetIncome = row.SafeString(1),
                    Bonus = row.SafeString(2),
                    CUTX = row.SafeString(3),
                    WorkExpenses = row.SafeString(4),
                    ChasePyN4 = row.SafeString(5),
                    Mortgage = row.SafeString(6),
                    Groceries = row.SafeString(7),
                    Other = row.SafeString(8),
                    Bills = row.SafeString(9),
                    CC = row.SafeString(10),
                    RegularLoans = row.SafeString(11),
                    ShortTerm = row.SafeString(12),
                    CalculatedTotalExpense = row.SafeString(13),
                    AfterExpenses = row.SafeString(14),
                    Source = row.SafeString(15),
                    Notes = row.SafeString(16),
                    TwentiyFivePercentBuffer = row.SafeString(17),
                    MaxUsableExtraSeventyFivePercent = row.SafeString(18)
                });
            }

            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.MORTGAGE_REFI_READINESS);
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
                Name = row.SafeString(0),
                Value = row.SafeString(1),
                Notes = row.SafeString(2)
            };
        }

        private OverallMetrics AddOverallMetric(IList<object> row)
        {
            return new OverallMetrics
            {
                DebtType = row.SafeString(0),
                Balance = row.SafeString(1),
                PercentageOfTotal = row.SafeString(2)
            };
        }
    }
}
