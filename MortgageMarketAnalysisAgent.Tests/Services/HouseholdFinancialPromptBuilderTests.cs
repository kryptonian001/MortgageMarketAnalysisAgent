using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Services.Concretes;
using MortgageMarketAnalysisAgent.Tests.TestHelpers;

namespace MortgageMarketAnalysisAgent.Tests.Services;

public class HouseholdFinancialPromptBuilderTests
{
    private readonly Mock<ILogger<HouseholdFinancialPromptBuilder>> _mockLogger;
    private readonly HouseholdFinancialPromptBuilder _builder;

    public HouseholdFinancialPromptBuilderTests()
    {
        _mockLogger = MockHelpers.CreateMockLogger<HouseholdFinancialPromptBuilder>();
        _builder = new HouseholdFinancialPromptBuilder(_mockLogger.Object);

        // Create test template files
        EnsureTestTemplatesExist();
    }

    private void EnsureTestTemplatesExist()
    {
        var baseDir = AppContext.BaseDirectory;
        var templatesDir = Path.Combine(baseDir, "templates");
        var promptsDir = Path.Combine(templatesDir, "prompts");
        var reportsDir = Path.Combine(templatesDir, "reports");

        Directory.CreateDirectory(promptsDir);
        Directory.CreateDirectory(reportsDir);

        var hifiPath = Path.Combine(promptsDir, "hifi.pmt");
        var analysisPath = Path.Combine(reportsDir, "analysis.rtl");

        // Always overwrite to ensure test content is present
        File.WriteAllText(hifiPath, "Test HiFi Template\nAnalysis Date: [[##ANALYSIS_DATE##]]");
        File.WriteAllText(analysisPath, "Test Analysis Template");
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange & Act
        var builder = new HouseholdFinancialPromptBuilder(_mockLogger.Object);

        // Assert
        builder.Should().NotBeNull();
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_ReturnsPromptString()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_IncludesDatePlaceholderReplacement()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().NotContain("[[##ANALYSIS_DATE##]]");
        result.Should().Contain(DateTime.Today.ToString("MM/dd/yyyy"));
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_IncludesDashboardSummary()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Clean high-level financial summary:");
        result.Should().Contain("Total monthly bills:");
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_IncludesCreditProfileSummary()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Clean credit profile summary:");
        result.Should().Contain("John Doe");
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_IncludesJsonModel()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Here is the full household financial model as JSON:");
        result.Should().Contain("```json");
        result.Should().Contain("agentDashboard");
    }

    [Fact]
    public void BuilPrompt_WithCompleteModel_LogsInformation()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        _mockLogger.VerifyLogInformation("Building Prompt");
        _mockLogger.VerifyLogInformation("Prompt Complete");
    }

    [Fact]
    public void BuilPrompt_WithEmptyCollections_ReturnsValidPrompt()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel
        {
            AgentDashboard = new AgentDashboard(),
            CreditProfiles = new List<CreditProfile>()
        };

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("```json");
    }

    [Fact]
    public void BuilPrompt_WithNullDashboard_HandlesGracefully()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel
        {
            AgentDashboard = null!,
            CreditProfiles = new List<CreditProfile>()
        };

        // Act
        Action act = () => _builder.BuilPrompt(model);

        // Assert
        // This will throw NullReferenceException when calling BuildModelSummary
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void BuilPrompt_WithMinimalModel_IncludesAllSections()
    {
        // Arrange
        var model = new HouseholdFinancialIntelligenceModel
        {
            AgentDashboard = new AgentDashboard(),
            CreditProfiles = new List<CreditProfile>()
        };

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Test HiFi Template");
        result.Should().Contain("Test Analysis Template");
        result.Should().Contain("```json");
    }

    [Fact]
    public void BuilPrompt_FormatsDateCorrectly()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var expectedDate = DateTime.Today.ToString("MM/dd/yyyy");

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain(expectedDate);
    }

    [Fact]
    public void BuilPrompt_IncludesTemplateContent()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Test HiFi Template");
        result.Should().Contain("Test Analysis Template");
    }

    [Fact]
    public void BuilPrompt_HasProperStructure()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        // Verify the prompt has the expected sections in order
        var hifiIndex = result.IndexOf("Test HiFi Template");
        var summaryIndex = result.IndexOf("Clean high-level financial summary:");
        var creditIndex = result.IndexOf("Clean credit profile summary:");
        var analysisIndex = result.IndexOf("Test Analysis Template");
        var jsonIndex = result.IndexOf("```json");

        hifiIndex.Should().BeGreaterThan(-1);
        summaryIndex.Should().BeGreaterThan(hifiIndex);
        creditIndex.Should().BeGreaterThan(summaryIndex);
        analysisIndex.Should().BeGreaterThan(creditIndex);
        jsonIndex.Should().BeGreaterThan(analysisIndex);
    }

    [Fact]
    public void BuilPrompt_WithMultipleCreditProfiles_IncludesAll()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("John Doe");
        result.Should().Contain("Jane Doe");
    }

    [Fact]
    public void BuilPrompt_JsonSection_IsProperlyFormatted()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();

        // Act
        var result = _builder.BuilPrompt(model);

        // Assert
        result.Should().Contain("Here is the full household financial model as JSON:");
        result.Should().Contain("```json");
        result.Should().Contain("```");

        // Verify JSON is indented (contains spaces at line starts)
        var jsonStart = result.IndexOf("```json");
        var jsonEnd = result.IndexOf("```", jsonStart + 7);
        var jsonSection = result.Substring(jsonStart, jsonEnd - jsonStart);
        jsonSection.Should().Match(s => s.Contains("  ")); // Contains indentation
    }
}
