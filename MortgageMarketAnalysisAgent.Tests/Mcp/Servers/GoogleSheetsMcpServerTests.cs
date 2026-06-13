using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Moq;
using MortgageMarketAnalysisAgent.Mcp.Models;
using MortgageMarketAnalysisAgent.Mcp.Servers;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Resilience;
using Polly;
using System.Text.Json;
using Xunit;

namespace MortgageMarketAnalysisAgent.Tests.Mcp.Servers;

public class GoogleSheetsMcpServerTests
{
    private readonly Mock<SheetsService> _mockSheetsService;
    private readonly Mock<ResiliencePipelineProvider> _mockResilienceProvider;
    private readonly Mock<ILogger<GoogleSheetsMcpServer>> _mockLogger;
    private readonly Mock<AgentConfig> _mockAgentConfig;
    private readonly GoogleSheetsMcpServer _mcpServer;

    public GoogleSheetsMcpServerTests()
    {
        _mockSheetsService = new Mock<SheetsService>();
        _mockResilienceProvider = new Mock<ResiliencePipelineProvider>();
        _mockLogger = new Mock<ILogger<GoogleSheetsMcpServer>>();
        _mockAgentConfig = new Mock<AgentConfig>();

        // Setup resilience pipeline to pass through
        var passThroughPipeline = new ResiliencePipelineBuilder<ValueRange>()
            .Build();
        _mockResilienceProvider
            .Setup(x => x.GetApiCallPipeline<ValueRange>())
            .Returns(passThroughPipeline);

        var spreadsheetPipeline = new ResiliencePipelineBuilder<Spreadsheet>()
            .Build();
        _mockResilienceProvider
            .Setup(x => x.GetApiCallPipeline<Spreadsheet>())
            .Returns(spreadsheetPipeline);

        _mcpServer = new GoogleSheetsMcpServer(
            _mockSheetsService.Object,
            _mockResilienceProvider.Object,
            _mockAgentConfig.Object,
            _mockLogger.Object);
    }

    #region Server Info Tests

    [Fact]
    public async Task GetServerInfoAsync_ReturnsCorrectMetadata()
    {
        // Act
        var serverInfo = await _mcpServer.GetServerInfoAsync();

        // Assert
        Assert.Equal("google-sheets-mcp-server", serverInfo.Name);
        Assert.Equal("1.0.0", serverInfo.Version);
        Assert.Equal("2024-11-05", serverInfo.ProtocolVersion);
        Assert.NotNull(serverInfo.Capabilities);
        Assert.NotNull(serverInfo.Capabilities.Resources);
        Assert.NotNull(serverInfo.Capabilities.Tools);
    }

    #endregion

    #region Resource Tests

    [Fact]
    public async Task ListResourcesAsync_Returns9ConfiguredSheets()
    {
        // Act
        var resources = await _mcpServer.ListResourcesAsync();

        // Assert
        Assert.Equal(9, resources.Count);
        Assert.Contains(resources, r => r.Name == "Credit Cards" && r.Uri.Contains("credit-cards"));
        Assert.Contains(resources, r => r.Name == "Income" && r.Uri.Contains("income"));
        Assert.Contains(resources, r => r.Name == "Monthly Bills" && r.Uri.Contains("monthly-bills"));
        Assert.Contains(resources, r => r.Name == "Loans" && r.Uri.Contains("loans"));
        Assert.All(resources, r =>
        {
            Assert.StartsWith("sheets://", r.Uri);
            Assert.Equal("application/json", r.MimeType);
            Assert.NotNull(r.Description);
        });
    }

    [Fact]
    public async Task ReadResourceAsync_WithValidUri_ReturnsJsonContent()
    {
        // Arrange
        var testData = new List<IList<object>>
        {
            new List<object> { "Card Name", "Balance", "Limit" },
            new List<object> { "Chase", "2500", "10000" },
            new List<object> { "Amex", "1200", "5000" }
        };

        var mockValueRange = new ValueRange { Values = testData };
        var mockRequest = new Mock<SpreadsheetsResource.ValuesResource.GetRequest>(
            _mockSheetsService.Object, 
            It.IsAny<string>(), 
            It.IsAny<string>());

        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockValueRange);

        _mockSheetsService
            .Setup(x => x.Spreadsheets.Values.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mockRequest.Object);

        // Act
        var content = await _mcpServer.ReadResourceAsync("sheets://test-sheet-id/credit-cards");

        // Assert
        Assert.NotNull(content);
        Assert.Equal("application/json", content.MimeType);
        Assert.NotNull(content.Text);

        var json = JsonDocument.Parse(content.Text);
        Assert.Equal("Credit Cards", json.RootElement.GetProperty("sheetName").GetString());
        Assert.Equal(3, json.RootElement.GetProperty("rowCount").GetInt32());
    }

    [Fact]
    public async Task ReadResourceAsync_WithInvalidUri_ThrowsMcpException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<McpException>(async () =>
            await _mcpServer.ReadResourceAsync("invalid-uri"));
    }

    [Fact]
    public async Task ReadResourceAsync_WithUnknownSheetKey_ThrowsMcpException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<McpException>(async () =>
            await _mcpServer.ReadResourceAsync("sheets://test-sheet-id/unknown-key"));

        Assert.Equal(McpErrorCode.ResourceNotFound, exception.ErrorCode);
        Assert.Contains("unknown-key", exception.Message);
    }

    #endregion

    #region Tool Tests

    [Fact]
    public async Task ListToolsAsync_Returns3Tools()
    {
        // Act
        var tools = await _mcpServer.ListToolsAsync();

        // Assert
        Assert.Equal(3, tools.Count);
        Assert.Contains(tools, t => t.Name == "read_sheet_range");
        Assert.Contains(tools, t => t.Name == "query_sheet_data");
        Assert.Contains(tools, t => t.Name == "get_sheet_metadata");
        Assert.All(tools, t =>
        {
            Assert.NotNull(t.Description);
            Assert.NotNull(t.InputSchema);
        });
    }

    [Fact]
    public async Task CallToolAsync_ReadSheetRange_WithValidArguments_ReturnsSuccess()
    {
        // Arrange
        var testData = new List<IList<object>>
        {
            new List<object> { "Header1", "Header2" },
            new List<object> { "Value1", "Value2" }
        };

        SetupMockSheetsService(testData);

        var arguments = new Dictionary<string, object>
        {
            ["sheetKey"] = "credit-cards",
            ["range"] = "A1:B2"
        };

        // Act
        var result = await _mcpServer.CallToolAsync("read_sheet_range", arguments);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Content);

        var json = JsonDocument.Parse(result.Content[0].Text!);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(2, json.RootElement.GetProperty("rowCount").GetInt32());
    }

    [Fact]
    public async Task CallToolAsync_QuerySheetData_ConvertsToStructuredJson()
    {
        // Arrange
        var testData = new List<IList<object>>
        {
            new List<object> { "Name", "Amount", "Status" },
            new List<object> { "Bill1", "100", "Paid" },
            new List<object> { "Bill2", "200", "Pending" }
        };

        SetupMockSheetsService(testData);

        var arguments = new Dictionary<string, object>
        {
            ["sheetKey"] = "monthly-bills",
            ["headerRow"] = JsonDocument.Parse("1").RootElement,
            ["includeHeaders"] = JsonDocument.Parse("true").RootElement
        };

        // Act
        var result = await _mcpServer.CallToolAsync("query_sheet_data", arguments);

        // Assert
        Assert.False(result.IsError);

        var json = JsonDocument.Parse(result.Content[0].Text!);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        var data = json.RootElement.GetProperty("data");
        Assert.Equal(2, data.GetArrayLength());

        var firstRow = data[0];
        Assert.Equal("Bill1", firstRow.GetProperty("Name").GetString());
        Assert.Equal("100", firstRow.GetProperty("Amount").GetString());
        Assert.Equal("Paid", firstRow.GetProperty("Status").GetString());
    }

    [Fact]
    public async Task CallToolAsync_WithUnknownTool_ThrowsMcpException()
    {
        // Arrange
        var arguments = new Dictionary<string, object>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<McpException>(async () =>
            await _mcpServer.CallToolAsync("unknown_tool", arguments));

        Assert.Equal(McpErrorCode.MethodNotFound, exception.ErrorCode);
    }

    [Fact]
    public async Task CallToolAsync_WithInvalidSheetKey_ReturnsError()
    {
        // Arrange
        var arguments = new Dictionary<string, object>
        {
            ["sheetKey"] = "invalid-key"
        };

        // Act
        var result = await _mcpServer.CallToolAsync("read_sheet_range", arguments);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Unknown sheet key", result.Content[0].Text);
    }

    [Fact]
    public async Task CallToolAsync_GetSheetMetadata_ReturnsSpreadsheetInfo()
    {
        // Arrange
        var mockSpreadsheet = new Spreadsheet
        {
            Properties = new SpreadsheetProperties
            {
                Title = "Test Spreadsheet",
                Locale = "en_US",
                TimeZone = "America/New_York"
            },
            Sheets = new List<Sheet>
            {
                new Sheet
                {
                    Properties = new SheetProperties
                    {
                        SheetId = 0,
                        Title = "Sheet1",
                        Index = 0,
                        GridProperties = new GridProperties
                        {
                            RowCount = 100,
                            ColumnCount = 10
                        }
                    }
                }
            }
        };

        var mockRequest = new Mock<SpreadsheetsResource.GetRequest>(
            _mockSheetsService.Object, 
            It.IsAny<string>());

        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpreadsheet);

        _mockSheetsService
            .Setup(x => x.Spreadsheets.Get(It.IsAny<string>()))
            .Returns(mockRequest.Object);

        var arguments = new Dictionary<string, object>
        {
            ["includeAllSheets"] = JsonDocument.Parse("false").RootElement
        };

        // Act
        var result = await _mcpServer.CallToolAsync("get_sheet_metadata", arguments);

        // Assert
        Assert.False(result.IsError);

        var json = JsonDocument.Parse(result.Content[0].Text!);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("Test Spreadsheet", json.RootElement.GetProperty("title").GetString());
    }

    #endregion

    #region Helper Methods

    private void SetupMockSheetsService(IList<IList<object>> testData)
    {
        var mockValueRange = new ValueRange { Values = testData };
        var mockRequest = new Mock<SpreadsheetsResource.ValuesResource.GetRequest>(
            _mockSheetsService.Object,
            It.IsAny<string>(),
            It.IsAny<string>());

        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockValueRange);

        _mockSheetsService
            .Setup(x => x.Spreadsheets.Values.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mockRequest.Object);
    }

    #endregion
}
