using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MortgageMarketAnalysisAgent.Agents.Interfaces;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Resilience;
using MortgageMarketAnalysisAgent.Services.Concretes;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using MortgageMarketAnalysisAgent.Tests.TestHelpers;

namespace MortgageMarketAnalysisAgent.Tests.Services;

public class MarketAnalysisServiceTests
{
    private readonly Mock<IReportBuildingService> _mockReportBuilder;
    private readonly Mock<IPromptBuilder> _mockPromptBuilder;
    private readonly Mock<IAgent> _mockAgent;
    private readonly Mock<INotify> _mockNotifier;
    private readonly Mock<ILogger<MarketAnalysisService>> _mockLogger;
    private readonly ResiliencePipelineProvider _resilienceProvider;
    private readonly IOptions<AgentConfig> _options;

    public MarketAnalysisServiceTests()
    {
        _mockReportBuilder = new Mock<IReportBuildingService>();

        _mockPromptBuilder = new Mock<IPromptBuilder>();
        _mockAgent = new Mock<IAgent>();
        _mockNotifier = new Mock<INotify>();
        _mockLogger = MockHelpers.CreateMockLogger<MarketAnalysisService>();

        // Create real ResiliencePipelineProvider for tests
        var resilienceLogger = MockHelpers.CreateMockLogger<ResiliencePipelineProvider>();
        _resilienceProvider = new ResiliencePipelineProvider(resilienceLogger.Object);

        _options = TestFixtures.CreateAgentConfigOptions();
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_CreatesInstanceWithNullEmail()
    {
        // Arrange & Act
        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            null!,
            _resilienceProvider,
            _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task RunAnalysis_WithValidData_ExecutesAllSteps()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var analysis = "Test analysis result";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(model);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(prompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(analysis);

        _mockNotifier
            .Setup(x => x.SendEmailNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockReportBuilder.Verify(x => x.BuildHouseholdFinancialIntelligenceReport(), Times.Once);
        _mockPromptBuilder.Verify(x => x.BuilPrompt(model), Times.Once);
        _mockAgent.Verify(x => x.RunAnalysisAsync(prompt), Times.Once);
        _mockNotifier.Verify(x => x.SendEmailNotificationAsync(
            "test@example.com",
            "Mortgage Refi Readiness Analysis",
            analysis), Times.Once);
    }

    [Fact]
    public async Task RunAnalysis_LogsAppropriateMessages()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var analysis = "Test analysis";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(model);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(prompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(analysis);

        _mockNotifier
            .Setup(x => x.SendEmailNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockLogger.VerifyLogInformation("Retrieving Household_Financial_Intelligence_Agent_Ready spreadsheet");
        _mockLogger.VerifyLogInformation("Building market analysis prompt");
        _mockLogger.VerifyLogInformation("Sending promp to ChatGPT");
        _mockLogger.VerifyLogInformation("Results:");
        _mockLogger.VerifyLogInformation("Sending to email:");
    }

    [Fact]
    public async Task RunAnalysis_CallsServicesInCorrectOrder()
    {
        // Arrange
        var callOrder = new List<string>();
        var model = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var analysis = "Test analysis";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(() =>
            {
                callOrder.Add("BuildReport");
                return model;
            });

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(() =>
            {
                callOrder.Add("BuildPrompt");
                return prompt;
            });

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(() =>
            {
                callOrder.Add("RunAnalysis");
                return analysis;
            });

        _mockNotifier
            .Setup(x => x.SendEmailNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(() =>
            {
                callOrder.Add("SendEmail");
                return Task.CompletedTask;
            });

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        callOrder.Should().Equal("BuildReport", "BuildPrompt", "RunAnalysis", "SendEmail");
    }

    [Fact]
    public async Task RunAnalysis_PassesCorrectEmailSubject()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var analysis = "Test analysis";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(model);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(prompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(analysis);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockNotifier.Verify(x => x.SendEmailNotificationAsync(
            It.IsAny<string>(),
            "Mortgage Refi Readiness Analysis",
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RunAnalysis_PassesAnalysisResultToNotifier()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var expectedAnalysis = "Detailed financial analysis with recommendations";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(model);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(prompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedAnalysis);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockNotifier.Verify(x => x.SendEmailNotificationAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            expectedAnalysis), Times.Once);
    }

    [Fact]
    public async Task RunAnalysis_WithEmptyEmailAddress_ThrowsInvalidOperationException()
    {
        // Arrange
        var configWithEmptyEmail = new AgentConfig
        {
            NotificationEmail = ""
        };
        var optionsWithEmptyEmail = Options.Create(configWithEmptyEmail);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            optionsWithEmptyEmail,
            _resilienceProvider,
            _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RunAnalysis());
        Assert.Contains("NotificationEmail is required", exception.Message);
    }

    [Fact]
    public async Task RunAnalysis_WithNullOptionsValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockOptionsWithNull = new Mock<IOptions<AgentConfig>>();
        mockOptionsWithNull.Setup(x => x.Value).Returns((AgentConfig)null!);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            mockOptionsWithNull.Object,
            _resilienceProvider,
            _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RunAnalysis());
        Assert.Contains("NotificationEmail is required", exception.Message);
    }

    [Fact]
    public async Task RunAnalysis_PassesModelToPromptBuilder()
    {
        // Arrange
        var expectedModel = TestFixtures.CreateCompleteFinancialModel();
        var prompt = "Test prompt";
        var analysis = "Test analysis";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(expectedModel);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(prompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(analysis);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockPromptBuilder.Verify(x => x.BuilPrompt(
            It.Is<HouseholdFinancialIntelligenceModel>(m => m == expectedModel)), 
            Times.Once);
    }

    [Fact]
    public async Task RunAnalysis_PassesPromptToAgent()
    {
        // Arrange
        var model = TestFixtures.CreateCompleteFinancialModel();
        var expectedPrompt = "Detailed financial analysis prompt with data";
        var analysis = "Test analysis";

        _mockReportBuilder
            .Setup(x => x.BuildHouseholdFinancialIntelligenceReport())
            .ReturnsAsync(model);

        _mockPromptBuilder
            .Setup(x => x.BuilPrompt(It.IsAny<HouseholdFinancialIntelligenceModel>()))
            .Returns(expectedPrompt);

        _mockAgent
            .Setup(x => x.RunAnalysisAsync(It.IsAny<string>()))
            .ReturnsAsync(analysis);

        var service = new MarketAnalysisService(
            _mockReportBuilder.Object,
            _mockPromptBuilder.Object,
            _mockAgent.Object,
            _mockNotifier.Object,
            _options,
            _resilienceProvider,
            _mockLogger.Object);

        // Act
        await service.RunAnalysis();

        // Assert
        _mockAgent.Verify(x => x.RunAnalysisAsync(expectedPrompt), Times.Once);
    }
}
