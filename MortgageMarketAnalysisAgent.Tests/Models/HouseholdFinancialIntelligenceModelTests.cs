using FluentAssertions;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;

namespace MortgageMarketAnalysisAgent.Tests.Models;

public class HouseholdFinancialIntelligenceModelTests
{
    [Fact]
    public void ReportCells_SheetId_IsNotEmpty()
    {
        // Assert
        HouseholdFinancialIntelligenceModel.ReportCells.SHEET_ID.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ReportCells_AllRanges_AreNotEmpty()
    {
        // Assert
        HouseholdFinancialIntelligenceModel.ReportCells.TOTAL_METRICS.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.OVERALL_METRICS.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.AGENT_PRIORITIES.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.INCOMES.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.MONTHLY_BILLS.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_CARDS.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.CREDIT_PROFILES.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.LOANS.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.SHORT_TERM_FINANCING.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.PAYCHECK_CASH_FLOW.Should().NotBeNullOrEmpty();
        HouseholdFinancialIntelligenceModel.ReportCells.MORTGAGE_REFI_READINESS.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_InitializesCollections()
    {
        // Act
        var model = new HouseholdFinancialIntelligenceModel();

        // Assert
        model.Incomes.Should().NotBeNull().And.BeEmpty();
        model.MonthlyBills.Should().NotBeNull().And.BeEmpty();
        model.CreditCards.Should().NotBeNull().And.BeEmpty();
        model.CreditProfiles.Should().NotBeNull().And.BeEmpty();
        model.Loans.Should().NotBeNull().And.BeEmpty();
        model.ShortTermFinancing.Should().NotBeNull().And.BeEmpty();
        model.PaychecCashFlow.Should().NotBeNull().And.BeEmpty();
        model.MortgageRefiReadinesses.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void AgentDashboard_CanBeSet()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel();
        var dashboard = new AgentDashboard();

        // Act
        model.AgentDashboard = dashboard;

        // Assert
        model.AgentDashboard.Should().BeSameAs(dashboard);
    }

    [Fact]
    public void Collections_CanAddItems()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel();

        // Act
        model.Incomes.Add(new Income());
        model.MonthlyBills.Add(new MonthlyBills());
        model.CreditCards.Add(new CreditCard());
        model.CreditProfiles.Add(new CreditProfile());
        model.Loans.Add(new Loan());
        model.ShortTermFinancing.Add(new ShortTermFinance());
        model.PaychecCashFlow.Add(new CashFlow());
        model.MortgageRefiReadinesses.Add(new MortgageRefiReadiness());

        // Assert
        model.Incomes.Should().HaveCount(1);
        model.MonthlyBills.Should().HaveCount(1);
        model.CreditCards.Should().HaveCount(1);
        model.CreditProfiles.Should().HaveCount(1);
        model.Loans.Should().HaveCount(1);
        model.ShortTermFinancing.Should().HaveCount(1);
        model.PaychecCashFlow.Should().HaveCount(1);
        model.MortgageRefiReadinesses.Should().HaveCount(1);
    }
}
