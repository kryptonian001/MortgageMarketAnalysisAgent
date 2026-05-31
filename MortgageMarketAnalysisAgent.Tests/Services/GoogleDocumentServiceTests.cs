using FluentAssertions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Moq;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Services.Concretes;
using MortgageMarketAnalysisAgent.Tests.TestHelpers;

namespace MortgageMarketAnalysisAgent.Tests.Services;

public class GoogleDocumentServiceTests
{
    private readonly Mock<ILogger<GoogleDocumentService>> _mockLogger;
    private readonly AgentConfig _config;

    public GoogleDocumentServiceTests()
    {
        _mockLogger = MockHelpers.CreateMockLogger<GoogleDocumentService>();
        _config = TestFixtures.CreateValidAgentConfig();
    }

    // Note: GoogleDocumentService requires UserCredential which cannot be easily mocked
    // because it has no parameterless constructor and depends on Google API infrastructure.
    // These tests would require integration testing or refactoring to inject abstractions.
    // For now, we test what we can at the integration boundaries.

    [Fact]
    public void ConfigStructure_ValidConfig_HasRequiredProperties()
    {
        // Assert
        _config.Should().NotBeNull();
        _config.ApplicationName.Should().NotBeNullOrEmpty();
        _config.GoogleConfigPath.Should().NotBeNull();
    }

    [Fact]
    public void AgentConfig_SupportsNullApplicationName()
    {
        // Arrange
        var configWithNull = new AgentConfig
        {
            ApplicationName = null!
        };

        // Assert
        // Config creation doesn't throw
        configWithNull.Should().NotBeNull();
    }

    [Fact]
    public void AgentConfig_SupportsEmptyApplicationName()
    {
        // Arrange
        var configWithEmpty = new AgentConfig
        {
            ApplicationName = ""
        };

        // Assert
        configWithEmpty.Should().NotBeNull();
        configWithEmpty.ApplicationName.Should().BeEmpty();
    }

    // Integration tests would go here if we had proper test credentials
    // For now, the service is tested indirectly through MarketAnalysisServiceTests
}
