using FluentAssertions;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Tests.TestHelpers;
using Newtonsoft.Json;

namespace MortgageMarketAnalysisAgent.Tests.Helpers;

public class TemplateHelperTests
{
    #region GetFullModelReport Tests

    [Fact]
    public void GetFullModelReport_WithCompleteModel_ReturnsValidJson()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = model.GetFullModelReport();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("agentDashboard");
        result.Should().Contain("incomes");
        result.Should().Contain("creditCards");
    }

    [Fact]
    public void GetFullModelReport_WithCompleteModel_ReturnsValidJsonStructure()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = model.GetFullModelReport();

        // Assert
        var deserialized = JsonConvert.DeserializeObject<HouseholdFinancialIntelligenceModel>(result);
        deserialized.Should().NotBeNull();
        deserialized!.AgentDashboard.Should().NotBeNull();
    }

    [Fact]
    public void GetFullModelReport_WithCompleteModel_UsesCamelCase()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = model.GetFullModelReport();

        // Assert
        result.Should().Contain("\"agentDashboard\"");
        result.Should().Contain("\"creditProfiles\"");
        result.Should().NotContain("\"AgentDashboard\"");
    }

    [Fact]
    public void GetFullModelReport_WithCompleteModel_IsIndented()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = model.GetFullModelReport();

        // Assert
        result.Should().Contain("  ");
        result.Should().Contain("\n");
    }

    [Fact]
    public void GetFullModelReport_WithEmptyCollections_ReturnsValidJson()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel
        {
            AgentDashboard = new AgentDashboard()
        };

        // Act
        var result = model.GetFullModelReport();

        // Assert
        result.Should().NotBeNullOrEmpty();
        var deserialized = JsonConvert.DeserializeObject<HouseholdFinancialIntelligenceModel>(result);
        deserialized.Should().NotBeNull();
    }

    #endregion

    #region GetTemplate Tests

    [Fact]
    public void GetTemplate_WithValidPath_ReturnsFileContent()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "TestHiFi.pmt");
        Directory.CreateDirectory(Path.GetDirectoryName(testFilePath)!);
        File.WriteAllText(testFilePath, "Test template content");

        try
        {
            // Act
            var result = TemplateHelper.GetTemplate(testFilePath);

            // Assert
            result.Should().Be("Test template content");
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }
    }

    [Fact]
    public void GetTemplate_WithMultilineContent_ReturnsAllLines()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "multiline.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(testFilePath)!);
        var content = "Line 1\nLine 2\nLine 3";
        File.WriteAllText(testFilePath, content);

        try
        {
            // Act
            var result = TemplateHelper.GetTemplate(testFilePath);

            // Assert
            result.Should().Be(content);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }
    }

    [Fact]
    public void GetTemplate_WithNonExistentFile_ThrowsException()
    {
        // Arrange
        var nonExistentPath = "non_existent_file.txt";

        // Act
        Action act = () => TemplateHelper.GetTemplate(nonExistentPath);

        // Assert
        act.Should().Throw<FileNotFoundException>();
    }

    #endregion

    #region BuildModelSummary Tests

    [Fact]
    public void BuildModelSummary_WithCompleteDashboard_ReturnsFormattedSummary()
    {
        // Arrange
        var dashboard = TestFixtures.CreateAgentDashboard();

        // Act
        var result = dashboard.BuildModelSummary();

        // Assert
        result.Should().Contain("Clean high-level financial summary:");
        result.Should().Contain("Total monthly bills:");
        result.Should().Contain("Total credit card balance:");
        result.Should().Contain("$3,500");
        result.Should().Contain("$5,000");
    }

    [Fact]
    public void BuildModelSummary_WithAllMetrics_IncludesAllFields()
    {
        // Arrange
        var dashboard = TestFixtures.CreateAgentDashboard();

        // Act
        var result = dashboard.BuildModelSummary();

        // Assert
        result.Should().Contain("Total monthly bills:");
        result.Should().Contain("Total credit card balance:");
        result.Should().Contain("Total credit card limit:");
        result.Should().Contain("Overall credit utilization:");
        result.Should().Contain("Highest utilization card:");
        result.Should().Contain("Highest utilization percentage:");
        result.Should().Contain("Total regular loan balance:");
        result.Should().Contain("Total short-term balance:");
        result.Should().Contain("Max extra allocation rule:");
        result.Should().Contain("Open data quality issues:");
    }

    [Fact]
    public void BuildModelSummary_WithNullMetrics_HandlesGracefully()
    {
        // Arrange
        var dashboard = new AgentDashboard
        {
            TotalMonthlyBills = null,
            TotalCreditCardBalance = null
        };

        // Act
        var result = dashboard.BuildModelSummary();

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Clean high-level financial summary:");
    }

    #endregion

    #region BuildCreditProfileSummary Tests

    [Fact]
    public void BuildCreditProfileSummary_WithMultipleProfiles_ReturnsFormattedSummary()
    {
        // Arrange
        var profiles = TestFixtures.CreateCreditProfiles();

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().Contain("Clean credit profile summary:");
        result.Should().Contain("John Doe");
        result.Should().Contain("Jane Doe");
        result.Should().Contain("FICO 8=750");
        result.Should().Contain("FICO 8=780");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithAllScoreTypes_IncludesAllFields()
    {
        // Arrange
        var profiles = TestFixtures.CreateCreditProfiles();

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().Contain("FICO 8=");
        result.Should().Contain("FICO 5=");
        result.Should().Contain("FICO 4=");
        result.Should().Contain("FICO 2=");
        result.Should().Contain("Mortgage middle score=");
        result.Should().Contain("Vantage 3.0=");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithValidScores_IncludesConservativeScore()
    {
        // Arrange
        var profiles = TestFixtures.CreateCreditProfiles();

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().Contain("Conservative household planning score: 748");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithSingleProfile_ReturnsFormattedSummary()
    {
        // Arrange
        var profiles = new List<CreditProfile>
        {
            new CreditProfile
            {
                Person = "Single Person",
                Fico8 = "750",
                Fico5 = "745",
                Fico4 = "748",
                Fico2 = "752",
                MortgageMiddleScore = 748,
                Vantage3_0 = "755",
                ScoreDate = "11/01/2024",
                DataConfidence = "High",
                Notes = "Test"
            }
        };

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().Contain("Single Person");
        result.Should().Contain("Conservative household planning score: 748");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithNoMortgageScores_DoesNotIncludeConservativeScore()
    {
        // Arrange
        var profiles = new List<CreditProfile>
        {
            new CreditProfile
            {
                Person = "Person",
                Fico8 = "750",
                MortgageMiddleScore = null
            }
        };

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().NotContain("Conservative household planning score:");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithEmptyList_ReturnsHeaderOnly()
    {
        // Arrange
        var profiles = new List<CreditProfile>();

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().Contain("Clean credit profile summary:");
        result.Should().NotContain("Conservative household planning score:");
    }

    [Fact]
    public void BuildCreditProfileSummary_WithZeroMortgageScore_DoesNotIncludeConservativeScore()
    {
        // Arrange
        var profiles = new List<CreditProfile>
        {
            new CreditProfile
            {
                Person = "Person",
                Fico8 = "750",
                MortgageMiddleScore = 0
            }
        };

        // Act
        var result = profiles.BuildCreditProfileSummary();

        // Assert
        result.Should().NotContain("Conservative household planning score:");
    }

    #endregion
}
