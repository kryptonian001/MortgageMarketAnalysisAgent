using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;

namespace MortgageMarketAnalysisAgent.Tests.TestHelpers;

public static class TestFixtures
{
    public static AgentConfig CreateValidAgentConfig()
    {
        return new AgentConfig
        {
            ApplicationName = "Test Application",
            GoogleConfigPath = "test_client_secret.json",
            NotificationEmail = "test@example.com",
            OpenAiKey = "sk-test-key-12345"
        };
    }

    public static IOptions<AgentConfig> CreateAgentConfigOptions()
    {
        return Options.Create(CreateValidAgentConfig());
    }

    public static HouseholdFinancialIntelligenceModel CreateCompleteFinancialModel()
    {
        return new HouseholdFinancialIntelligenceModel
        {
            AgentDashboard = CreateAgentDashboard(),
            Incomes = CreateIncomes(),
            MonthlyBills = CreateMonthlyBills(),
            CreditCards = CreateCreditCards(),
            CreditProfiles = CreateCreditProfiles(),
            Loans = CreateLoans(),
            ShortTermFinancing = CreateShortTermFinancing(),
            PaychecCashFlow = CreateCashFlows(),
            MortgageRefiReadinesses = CreateMortgageRefiReadiness()
        };
    }

    public static AgentDashboard CreateAgentDashboard()
    {
        return new AgentDashboard
        {
            TotalMonthlyBills = new DashboardMetric { Name = "Total Monthly Bills", Value = "$3,500", Notes = "All bills" },
            TotalCreditCardBalance = new DashboardMetric { Name = "Total CC Balance", Value = "$5,000", Notes = "" },
            TotalCreditCardLimit = new DashboardMetric { Name = "Total CC Limit", Value = "$25,000", Notes = "" },
            OverallCreditUtilization = new DashboardMetric { Name = "Credit Utilization", Value = "20%", Notes = "" },
            HighestUtilizationCard = new DashboardMetric { Name = "Highest Card", Value = "Chase Visa", Notes = "" },
            HightestUtilization = new DashboardMetric { Name = "Highest %", Value = "45%", Notes = "" },
            TotalRegularLoanBalance = new DashboardMetric { Name = "Loan Balance", Value = "$250,000", Notes = "" },
            TotalShortTermBalance = new DashboardMetric { Name = "Short Term", Value = "$1,200", Notes = "" },
            MaxExtraAllocationRule = new DashboardMetric { Name = "Max Extra", Value = "$500", Notes = "" },
            OpenDataQualityIssues = new DashboardMetric { Name = "Data Issues", Value = "2", Notes = "Missing dates" },
            CreditCards = new OverallMetrics { DebtType = "Credit Cards", Balance = "$5,000", PercentageOfTotal = "2%" },
            RegularLoans = new OverallMetrics { DebtType = "Regular Loans", Balance = "$250,000", PercentageOfTotal = "96%" },
            ShortTermFinancing = new OverallMetrics { DebtType = "Short Term", Balance = "$1,200", PercentageOfTotal = "2%" },
            AgentRunPriorities = new List<AgentRunPriority>
            {
                new AgentRunPriority { Priority = "1", Name = "Credit Utilization", Description = "Reduce to under 10%" },
                new AgentRunPriority { Priority = "2", Name = "Cash Buffer", Description = "Maintain 25% buffer" }
            }
        };
    }

    public static List<Income> CreateIncomes()
    {
        return new List<Income>
        {
            new Income
            {
                Person = "John Doe",
                PayFrequency = "Bi-Weekly",
                GrossPay = "$3,500",
                NetPay = "$2,800",
                NextPayDate = "12/15/2024",
                Bonus = "$0",
                Source = "Employer",
                Notes = "Regular paycheck",
                DataConfidence = "High"
            }
        };
    }

    public static List<MonthlyBills> CreateMonthlyBills()
    {
        return new List<MonthlyBills>
        {
            new MonthlyBills
            {
                BillName = "Mortgage",
                DueDate = "1",
                Amount = "$2,000",
                PayWindow = "1-5",
                Category = "Housing",
                PaymentAccount = "Checking",
                Weight = "High",
                Required = "Yes",
                Source = "Lender Statement",
                Notes = "Fixed rate"
            }
        };
    }

    public static List<CreditCard> CreateCreditCards()
    {
        return new List<CreditCard>
        {
            new CreditCard
            {
                CardName = "Chase Visa",
                DueDay = "15",
                MinimumPayment = "$50",
                Adjustment = "$0",
                RealAmount = "$50",
                Balance = "$2,500",
                Limit = "$10,000",
                Untilization = "25%",
                PaymentPct = "2%",
                PayoffTarget = "06/2025",
                StatementCloseDate = "10",
                APR = "18.99%",
                Owner = "John Doe",
                Source = "Statement",
                Notes = "Primary card"
            }
        };
    }

    public static List<CreditProfile> CreateCreditProfiles()
    {
        return new List<CreditProfile>
        {
            new CreditProfile
            {
                Person = "John Doe",
                Fico8 = "750",
                Fico5 = "748",
                Fico4 = "745",
                Fico2 = "752",
                Vantage3_0 = "755",
                ScoreDate = "11/01/2024",
                MortgageMiddleScore = 748,
                Notes = "Good credit",
                DataConfidence = "High"
            },
            new CreditProfile
            {
                Person = "Jane Doe",
                Fico8 = "780",
                Fico5 = "775",
                Fico4 = "778",
                Fico2 = "776",
                Vantage3_0 = "782",
                ScoreDate = "11/01/2024",
                MortgageMiddleScore = 776,
                Notes = "Excellent credit",
                DataConfidence = "High"
            }
        };
    }

    public static List<Loan> CreateLoans()
    {
        return new List<Loan>
        {
            new Loan
            {
                LoanName = "Mortgage",
                DueDay = "1",
                OriginalPayment = "$2,000",
                Adjustment = "$0",
                MonthlyPayment = "$2,000",
                Balance = "$250,000",
                StarteDate = "01/01/2020",
                EndDate = "01/01/2050",
                Total = "$360,000",
                PIF = "No",
                APR = "3.5%",
                Owner = "John & Jane Doe",
                Priority = "1",
                Source = "Lender",
                Notes = "30-year fixed"
            }
        };
    }

    public static List<ShortTermFinance> CreateShortTermFinancing()
    {
        return new List<ShortTermFinance>
        {
            new ShortTermFinance
            {
                Name = "Holiday Fund",
                DueDay = "15",
                Payment = "$200",
                Frequency = "Monthly",
                Balance = "$1,200",
                PayOff = "06/2025",
                RateAPR = "0%",
                StarteDate = "01/2024",
                EndDate = "06/2025",
                Source = "Personal Budget",
                Notes = "Interest-free"
            }
        };
    }

    public static List<CashFlow> CreateCashFlows()
    {
        return new List<CashFlow>
        {
            new CashFlow
            {
                PayDate = "12/15/2024",
                NetIncome = "$2,800",
                Bonus = "$0",
                CUTX = "$100",
                WorkExpenses = "$50",
                ChasePyN4 = "$0",
                Mortgage = "$2,000",
                Groceries = "$400",
                Other = "$100",
                Bills = "$300",
                CC = "$50",
                RegularLoans = "$0",
                ShortTerm = "$200",
                CalculatedTotalExpense = "$3,200",
                AfterExpenses = "$-400",
                Owner = "Paycheck",
                Notes = "Tight month",
                TwentiyFivePercentBuffer = "$700",
                MaxUsableExtraSeventyFivePercent = "$525"
            }
        };
    }

    public static List<MortgageRefiReadiness> CreateMortgageRefiReadiness()
    {
        return new List<MortgageRefiReadiness>
        {
            new MortgageRefiReadiness
            {
                Input = "Current Rate",
                Value = "3.5",
                Unit = "%",
                Source = "Lender",
                Status = "Current",
                Notes = "Fixed rate",
                AgentUse = "Comparison",
                LastUpdated = "11/01/2024"
            }
        };
    }

    public static IList<IList<object>> CreateGoogleSheetRow(params object[] values)
    {
        return new List<IList<object>> { new List<object>(values) };
    }

    public static IList<IList<object>> CreateGoogleSheetRows(int rowCount, int columnCount)
    {
        var rows = new List<IList<object>>();
        for (int i = 0; i < rowCount; i++)
        {
            var row = new List<object>();
            for (int j = 0; j < columnCount; j++)
            {
                row.Add($"Value_{i}_{j}");
            }
            rows.Add(row);
        }
        return rows;
    }
}
