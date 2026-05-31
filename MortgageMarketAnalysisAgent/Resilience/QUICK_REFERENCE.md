# Resilience Quick Reference Card

## 🚀 Quick Start

### 1. Add to Constructor
```csharp
private readonly ResiliencePipelineProvider _resilienceProvider;

public MyService(ResiliencePipelineProvider resilienceProvider)
{
	_resilienceProvider = resilienceProvider;
}
```

### 2. Wrap External Calls
```csharp
var pipeline = _resilienceProvider.GetApiCallPipeline<TResult>();

var result = await pipeline.ExecuteAsync(async cancellationToken =>
{
	return await _externalService.CallAsync(cancellationToken);
}, CancellationToken.None);
```

## 📦 Pipeline Types

| Pipeline | Timeout | Retries | Circuit Breaker | Use For |
|----------|---------|---------|-----------------|---------|
| **API Call** | 30s | 3 (2s, 4s, 8s) | Yes (50%, 30s) | Google APIs, OpenAI |
| **Notification** | 15s | 2 (1s, 2s) | No | Email, Alerts |
| **Critical** | 60s | 0 | No | Non-retryable ops |

## 🎯 Usage Examples

### Google Sheets
```csharp
var pipeline = _resilienceProvider.GetApiCallPipeline<ValueRange>();
var response = await pipeline.ExecuteAsync(async ct =>
	await sheetsService.Spreadsheets.Values.Get(id, range).ExecuteAsync(ct),
	CancellationToken.None);
```

### OpenAI
```csharp
var pipeline = _resilienceProvider.GetApiCallPipeline<ChatCompletion>();
var completion = await pipeline.ExecuteAsync(async ct =>
	await chatClient.CompleteChatAsync([new UserChatMessage(prompt)], ct),
	CancellationToken.None);
```

### Gmail
```csharp
var pipeline = _resilienceProvider.GetNotificationPipeline<Message>();
await pipeline.ExecuteAsync(async ct =>
	await gmailService.Users.Messages.Send(message, "me").ExecuteAsync(ct),
	CancellationToken.None);
```

## ⚠️ Exception Handling

```csharp
try
{
	await pipeline.ExecuteAsync(async ct => /* ... */, CancellationToken.None);
}
catch (BrokenCircuitException ex)
{
	_logger.LogError("Service unavailable - circuit breaker open");
}
catch (TimeoutException ex)
{
	_logger.LogError("Operation timed out");
}
catch (HttpRequestException ex)
{
	_logger.LogError("Network error");
}
```

## 📊 What Gets Logged

- ⚠️ **Retry attempts**: Warning with attempt number and delay
- ❌ **Circuit opened**: Error when circuit breaks
- ✅ **Circuit closed**: Info when circuit recovers
- 🔄 **Circuit half-open**: Info when testing service

## ✅ Best Practices

1. ✅ Pass `CancellationToken` through pipeline
2. ✅ Use appropriate pipeline type
3. ✅ Wrap only external calls, not entire methods
4. ✅ Log at appropriate levels
5. ✅ Handle `BrokenCircuitException` gracefully

## ❌ Common Mistakes

1. ❌ Forgetting to pass `CancellationToken`
2. ❌ Wrapping database calls (use different patterns)
3. ❌ Using API pipeline for notifications (too aggressive)
4. ❌ Ignoring circuit breaker exceptions
5. ❌ Setting timeout too low

## 🔧 Need to Customize?

Edit `ResiliencePipelineProvider.cs`:
- Change timeout values
- Adjust retry counts/delays
- Modify circuit breaker thresholds
- Add new pipeline types

## 📚 Learn More

- **Detailed Guide**: `MortgageMarketAnalysisAgent/Resilience/README.md`
- **Examples**: `MortgageMarketAnalysisAgent/Resilience/IMPLEMENTATION_EXAMPLES.md`
- **Working Code**: `MarketAnalysisService.cs`
- **Polly Docs**: https://www.pollydocs.org/

---

**Version**: 1.0 | **Polly**: v8.6.6 | **.NET**: 10.0
