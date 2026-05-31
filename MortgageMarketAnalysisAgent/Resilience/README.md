# Resilience Implementation Guide

## Overview
This application now uses **Polly v8** resilience pipelines to handle transient failures, timeouts, and circuit breaking for external service calls.

## What Was Added

### 1. ResiliencePipelineProvider
Located in `MortgageMarketAnalysisAgent/Resilience/ResiliencePipelineProvider.cs`

Provides three types of resilience pipelines:

#### **API Call Pipeline** (Primary)
- **Timeout**: 30 seconds
- **Retry**: 3 attempts with exponential backoff (2s, 4s, 8s) + jitter
- **Circuit Breaker**: Opens after 50% failure rate (min 3 requests), breaks for 30s
- **Use for**: Google APIs, OpenAI calls

#### **Notification Pipeline**
- **Timeout**: 15 seconds
- **Retry**: 2 attempts with exponential backoff
- **Use for**: Email notifications, non-critical alerts

#### **Critical Operation Pipeline**
- **Timeout**: 60 seconds only (no retries)
- **Use for**: Operations that must not be retried

## How to Use Resilience

### In Services (Already Applied)
`MarketAnalysisService` now uses resilience pipelines:

```csharp
private readonly ResiliencePipelineProvider _resilienceProvider;

public MarketAnalysisService(ResiliencePipelineProvider resilienceProvider, ...)
{
	_resilienceProvider = resilienceProvider;
}

public async Task RunAnalysis()
{
	var pipeline = _resilienceProvider.GetApiCallPipeline();

	await pipeline.ExecuteAsync(async cancellationToken =>
	{
		// Your code here - automatically protected by retry, timeout, circuit breaker
		var data = await _externalService.GetDataAsync();
		return data;
	}, CancellationToken.None);
}
```

### Adding to Other Services

#### Example: GoogleDocumentService
```csharp
public class GoogleDocumentService : IExternalDocumentService
{
	private readonly ResiliencePipelineProvider _resilienceProvider;

	public GoogleDocumentService(
		UserCredential credential, 
		AgentConfig config, 
		ResiliencePipelineProvider resilienceProvider,
		ILogger<GoogleDocumentService> logger)
	{
		_resilienceProvider = resilienceProvider;
		// ... existing initialization
	}

	public async Task<IList<IList<object>>> ReadRangeAsync(string sheetId, string range)
	{
		var pipeline = _resilienceProvider.GetApiCallPipeline<ValueRange>();

		var response = await pipeline.ExecuteAsync(async cancellationToken =>
		{
			var request = sheetsService.Spreadsheets.Values.Get(sheetId, range);
			return await request.ExecuteAsync(cancellationToken);
		}, CancellationToken.None);

		return response.Values ?? new List<IList<object>>();
	}
}
```

#### Example: MarketAnalysisAgent (OpenAI)
```csharp
public class MarketAnalysisAgent : IAgent
{
	private readonly ResiliencePipelineProvider _resilienceProvider;

	public MarketAnalysisAgent(
		IOptions<AgentConfig> options,
		ResiliencePipelineProvider resilienceProvider)
	{
		_resilienceProvider = resilienceProvider;
		// ... existing initialization
	}

	public async Task<string> RunAnalysisAsync(string mortgageReadiness)
	{
		var pipeline = _resilienceProvider.GetApiCallPipeline<ChatCompletion>();

		var completion = await pipeline.ExecuteAsync(async cancellationToken =>
		{
			return await chatClient.CompleteChatAsync(
				[new UserChatMessage(mortgageReadiness)],
				cancellationToken: cancellationToken);
		}, CancellationToken.None);

		return completion.Content[0].Text;
	}
}
```

## Exception Handling

The resilience pipelines handle these exceptions automatically:
- `HttpRequestException` - Network errors
- `TaskCanceledException` - Timeouts
- `TimeoutException` - Explicit timeout
- `Google.GoogleApiException` - Google API errors (429, 503, 504 status codes)

Additional exceptions to catch in your services:
- `Polly.CircuitBreaker.BrokenCircuitException` - Circuit breaker is open
- Handle gracefully and log appropriately

## Configuration (Future Enhancement)

To make resilience configurable, add to `appsettings.json`:

```json
{
  "Resilience": {
	"ApiCall": {
	  "TimeoutSeconds": 30,
	  "MaxRetryAttempts": 3,
	  "RetryDelaySeconds": 2,
	  "CircuitBreakerFailureRatio": 0.5,
	  "CircuitBreakerBreakDurationSeconds": 30
	},
	"Notification": {
	  "TimeoutSeconds": 15,
	  "MaxRetryAttempts": 2
	}
  }
}
```

Then update `ResiliencePipelineProvider` to read from configuration.

## Monitoring & Observability

All resilience events are logged:
- **Retry attempts**: Warning level with attempt number and delay
- **Circuit breaker opened**: Error level
- **Circuit breaker closed/half-opened**: Information level
- **Timeouts**: Captured as exceptions

Consider adding:
1. **Metrics**: Track retry count, circuit breaker state, timeout frequency
2. **Application Insights**: For Azure deployments
3. **Health checks**: Expose circuit breaker state via health endpoints

## Best Practices

1. ✅ **DO** wrap all external service calls (Google APIs, OpenAI, HTTP clients)
2. ✅ **DO** use appropriate pipeline for operation type
3. ✅ **DO** pass CancellationToken through the pipeline
4. ✅ **DO** log resilience events for debugging
5. ❌ **DON'T** wrap database calls (use different patterns)
6. ❌ **DON'T** retry non-idempotent operations without careful design
7. ❌ **DON'T** ignore BrokenCircuitException - handle gracefully

## Testing Resilience

### Unit Testing with Polly
```csharp
[Fact]
public async Task RunAnalysis_RetriesOnTransientFailure()
{
	// Arrange
	var mockService = new Mock<IExternalService>();
	mockService.SetupSequence(x => x.GetDataAsync())
		.ThrowsAsync(new HttpRequestException("Network error"))
		.ThrowsAsync(new HttpRequestException("Network error"))
		.ReturnsAsync(new Data()); // Succeeds on 3rd attempt

	var sut = new YourService(mockService.Object, resilienceProvider);

	// Act
	var result = await sut.ProcessAsync();

	// Assert
	Assert.NotNull(result);
	mockService.Verify(x => x.GetDataAsync(), Times.Exactly(3));
}
```

### Integration Testing
Simulate failures using chaos engineering tools or test endpoints that return specific HTTP status codes.

## Additional Resources

- [Polly Documentation](https://www.pollydocs.org/)
- [.NET Resilience](https://learn.microsoft.com/en-us/dotnet/core/resilience/)
- [Circuit Breaker Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)
