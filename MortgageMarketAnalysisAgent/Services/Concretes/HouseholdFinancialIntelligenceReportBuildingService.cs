using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HouseholdFinancialIntelligenceReportBuildingService> _logger;

        public HouseholdFinancialIntelligenceReportBuildingService(GoogleDocumentService documentService, ILogger<HouseholdFinancialIntelligenceReportBuildingService> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<HouseholdFinancialIntelligenceModel> BuildHouseholdFinancialIntelligenceReport()
        {
            HouseholdFinancialIntelligenceModel report = new HouseholdFinancialIntelligenceModel { AgentDashboard = new AgentDashboard() };

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.TOTAL_METRICS));
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

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.OVERALL_METRICS));
            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.OVERALL_METRICS);

            report.AgentDashboard.CreditCards = AddOverallMetric(rows[0]);
            report.AgentDashboard.RegularLoans = AddOverallMetric(rows[1]);
            report.AgentDashboard.ShortTermFinancing = AddOverallMetric(rows[0]);

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.AGENT_PRIORITIES));
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

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.INCOMES));
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

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.MONTHLY_BILLS));
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
                    Owner = row.SafeString(8),
                    Notes = row.SafeString(9)
                });
            }

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_CARDS));
            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_CARDS);
            foreach (var row in rows)
            {
                report.CreditCards.Add(new CreditCard
                {
                    CardName = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    MinimumPayment = row.SafeString(2),
                    //Adjustment = row.SafeString(3),
                    //RealAmount = row.SafeString(4),
                    Balance = row.SafeString(3),
                    Limit = row.SafeString(4),
                    Untilization = row.SafeString(5),
                    PaymentPct = row.SafeString(6),
                    PayoffTarget = row.SafeString(7),
                    StatementCloseDate = row.SafeString(8),
                    APR = row.SafeString(9),
                    Owner = row.SafeString(10),
                    Source = row.SafeString(11),
                    Notes = row.SafeString(12)
                });
            }

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_PROFILES));
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

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.LOANS));
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
                    Owner = row.SafeString(11),
                    Priority = row.SafeString(12),
                    Source = row.SafeString(13),
                    Notes = row.SafeString(14)
                });
            }

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.SHORT_TERM_FINANCING));
            rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.SHORT_TERM_FINANCING);
            foreach (var row in rows)
            {
                report.ShortTermFinancing.Add(new ShortTermFinance
                {
                    Name = row.SafeString(0),
                    DueDay = row.SafeString(1),
                    Payment = row.SafeString(2),
                    Frequency = row.SafeString(3),
                    Balance = row.SafeString(4),
                    PayOff = row.SafeString(5),
                    RateAPR = row.SafeString(6),
                    StarteDate = row.SafeString(7),
                    EndDate = row.SafeString(8),
                    Owner = row.SafeString(9),
                    Source = row.SafeString(10),
                    Notes = row.SafeString(11)
                });
            }

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.PAYCHECK_CASH_FLOW));
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
                    //ChasePyN4 = row.SafeString(5),
                    Mortgage = row.SafeString(5),
                    Groceries = row.SafeString(6),
                    //Other = row.SafeString(8),
                    //Bills = row.SafeString(9),
                    //CC = row.SafeString(10),
                    //RegularLoans = row.SafeString(11),
                    //ShortTerm = row.SafeString(12),
                    //CalculatedTotalExpense = row.SafeString(13),
                    //AfterExpenses = row.SafeString(14),
                    Owner = row.SafeString(7),
                    Notes = row.SafeString(8),
                    //TwentiyFivePercentBuffer = row.SafeString(17),
                    //MaxUsableExtraSeventyFivePercent = row.SafeString(18)
                });
            }

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.MORTGAGE_REFI_READINESS));
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

        public async Task<List<HousingMarketModel>> BuildMarketHouseReport()
        {
            List<HousingMarketModel> houses = new List<HousingMarketModel>();

            _logger.LogInformation(nameof(HouseholdFinancialIntelligenceModel.ReportCells.HOUSING_MARKET));
            var rows = await _documentService.ReadRangeAsync(HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID, HouseholdFinancialIntelligenceModel.ReportCells.HOUSING_MARKET);

            foreach (var row in rows)
            {
                houses.Add(new HousingMarketModel
                {                    
                    StreetAddress = row.SafeString(0),
                    PostalCode = row.SafeString(1),
                    BedRooms = row.SafeInt(2),
                    BathRooms = row.SafeDouble(3),
                    SquareFootage = row.SafeDouble(4)
                });
            }

            return houses;
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
