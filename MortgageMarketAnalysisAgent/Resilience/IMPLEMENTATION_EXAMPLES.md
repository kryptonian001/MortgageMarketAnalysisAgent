# Example: Adding Resilience to Your Other Services

This file shows you exactly how to add resilience to your remaining services.

## 1. GoogleDocumentService

### Before (Current)
```csharp
public async Task<IList<IList<object>>> ReadRangeAsync(string sheetId, string range)
{
	_logger.LogInformation($"Pulling data form range: {range}");
	SpreadsheetsResource.ValuesResource.GetRequest request =
		 sheetsService.Spreadsheets.Values.Get(sheetId, range);

	ValueRange response = await request.ExecuteAsync();

	return response.Values ?? default(IList<IList<object>>);
}
```

### After (With Resilience)
```csharp
public class GoogleDocumentService : IExternalDocumentService
{
	private readonly DocsService docsService;
	private readonly SheetsService sheetsService;
	private readonly ResiliencePipelineProvider _resilienceProvider;
	private readonly ILogger<GoogleDocumentService> _logger;

	public GoogleDocumentService(
		UserCredential credential, 
		AgentConfig? config,
		ResiliencePipelineProvider resilienceProvider,
		ILogger<GoogleDocumentService> logger)
	{
		var init = new BaseClientService.Initializer
		{
			HttpClientInitializer = credential,
			ApplicationName = config?.ApplicationName ?? ""
		};

		docsService = new DocsService(init);
		sheetsService = new SheetsService(init);
		_resilienceProvider = resilienceProvider;
		_logger = logger;
	}

	public async Task<IList<IList<object>>> ReadRangeAsync(string sheetId, string range)
	{
		_logger.LogInformation($"Pulling data form range: {range}");

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

### Update ServiceCollectionExtensions
```csharp
services.AddTransient<GoogleDocumentService>((sp) => 
	new GoogleDocumentService(
		creds, 
		googleClientCfg,
		sp.GetRequiredService<ResiliencePipelineProvider>(),
		sp.GetRequiredService<ILogger<GoogleDocumentService>>()));
```

---

## 2. MarketAnalysisAgent (OpenAI)

### Before (Current)
```csharp
public class MarketAnalysisAgent : IAgent
{
	private readonly ChatClient chatClient;

	public MarketAnalysisAgent(IOptions<AgentConfig> options)
	{
		ArgumentNullException.ThrowIfNull(options?.Value);
		var config = options.Value;
		chatClient = new ChatClient("gpt-4.1-mini", config.OpenAiKey);
	}

	public async Task<string> RunAnalysisAsync(string mortgageReadiness)
	{
		ChatCompletion completion = await chatClient.CompleteChatAsync(
		[
			new UserChatMessage(mortgageReadiness)
		]);

		return completion.Content[0].Text;
	}
}
```

### After (With Resilience)
```csharp
public class MarketAnalysisAgent : IAgent
{
	private readonly ChatClient chatClient;
	private readonly ResiliencePipelineProvider _resilienceProvider;

	public MarketAnalysisAgent(
		IOptions<AgentConfig> options,
		ResiliencePipelineProvider resilienceProvider)
	{
		ArgumentNullException.ThrowIfNull(options?.Value);

		var config = options.Value;
		chatClient = new ChatClient("gpt-4.1-mini", config.OpenAiKey);
		_resilienceProvider = resilienceProvider;
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

### Update ServiceCollectionExtensions
```csharp
services.AddTransient<IAgent, MarketAnalysisAgent>();
// No change needed - already uses DI constructor
```

---

## 3. GoogleNotificationService (Email)

### Before (Current)
```csharp
public async Task SendEmailNotificationAsync(string to, string subject, string body)
{
	try
	{
		// ... email sending logic
		await gmailService.Users.Messages.Send(message, "me").ExecuteAsync();
	}
	catch (Exception ex)
	{
		_logger.LogError(ex, "Failed to send email");
		throw;
	}
}
```

### After (With Resilience)
```csharp
public class GoogleNotificationService : INotify
{
	private readonly GmailService gmailService;
	private readonly ResiliencePipelineProvider _resilienceProvider;
	private readonly ILogger<GoogleNotificationService> _logger;

	public GoogleNotificationService(
		UserCredential credential,
		AgentConfig config,
		ResiliencePipelineProvider resilienceProvider,
		ILogger<GoogleNotificationService> logger)
	{
		var init = new BaseClientService.Initializer
		{
			HttpClientInitializer = credential,
			ApplicationName = config.ApplicationName
		};

		gmailService = new GmailService(init);
		_resilienceProvider = resilienceProvider;
		_logger = logger;
	}

	public async Task SendEmailNotificationAsync(string to, string subject, string body)
	{
		// Use notification pipeline (fewer retries for non-critical operations)
		var pipeline = _resilienceProvider.GetNotificationPipeline<Message>();

		try
		{
			await pipeline.ExecuteAsync(async cancellationToken =>
			{
				var message = CreateEmailMessage(to, subject, body);
				return await gmailService.Users.Messages
					.Send(message, "me")
					.ExecuteAsync(cancellationToken);
			}, CancellationToken.None);

			_logger.LogInformation("Email sent successfully to {To}", to);
		}
		catch (Polly.CircuitBreaker.BrokenCircuitException ex)
		{
			_logger.LogError(ex, "Circuit breaker open - Gmail service unavailable");
			throw;
		}
		catch (TimeoutException ex)
		{
			_logger.LogError(ex, "Email send timed out");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send email");
			throw;
		}
	}
}
```

### Update ServiceCollectionExtensions
```csharp
services.AddTransient<INotify, GoogleNotificationService>((sp) => 
	new GoogleNotificationService(
		creds, 
		googleClientCfg,
		sp.GetRequiredService<ResiliencePipelineProvider>(),
		sp.GetRequiredService<ILogger<GoogleNotificationService>>()));
```

---

## 4. HouseholdFinancialIntelligenceReportBuildingService

If this service uses `GoogleDocumentService`, it will automatically benefit from resilience once you update `GoogleDocumentService`. No changes needed unless it makes direct external calls.

---

## Complete ServiceCollectionExtensions Update

```csharp
public static async Task AddAgentConfigurationAsync(this IServiceCollection services)
{
	var builder = new ConfigurationBuilder();

	var config = builder.AddUserSecrets<Program>()
						.AddJsonFile(Path.Combine(AppContext.BaseDirectory,"appsettings.json"))
						.AddEnvironmentVariables()
						.Build();

	var cfg = config.GetSection("AgentConfig");
	googleClientCfg = cfg.Get<AgentConfig>();
	services.Configure<AgentConfig>(cfg);

	// Register Resilience Pipeline Provider (ALREADY DONE)
	services.AddSingleton<ResiliencePipelineProvider>();

	services.AddTransient<IMarketAnalysisService, MarketAnalysisService>();

	var creds = await GetGoogleCredentials();

	// Update GoogleDocumentService with ResiliencePipelineProvider
	services.AddTransient<GoogleDocumentService>((sp) => 
		new GoogleDocumentService(
			creds, 
			googleClientCfg,
			sp.GetRequiredService<ResiliencePipelineProvider>(),
			sp.GetRequiredService<ILogger<GoogleDocumentService>>()));

	// Update GoogleNotificationService with ResiliencePipelineProvider
	services.AddTransient<INotify, GoogleNotificationService>((sp) => 
		new GoogleNotificationService(
			creds, 
			googleClientCfg,
			sp.GetRequiredService<ResiliencePipelineProvider>(),
			sp.GetRequiredService<ILogger<GoogleNotificationService>>()));

	services.AddTransient<HouseholdFinancialIntelligenceReportBuildingService>();
	services.AddTransient<IPromptBuilder, HouseholdFinancialPromptBuilder>();

	// MarketAnalysisAgent already uses DI constructor
	services.AddTransient<IAgent, MarketAnalysisAgent>();
}
```

---

## Testing Your Changes

After updating each service, run:

```bash
dotnet build
dotnet test
```

Expected results:
- ✅ All builds succeed
- ✅ All tests pass
- ✅ Services now protected by resilience pipelines

---

## Verification Checklist

- [ ] `GoogleDocumentService` - Added ResiliencePipelineProvider
- [ ] `MarketAnalysisAgent` - Added ResiliencePipelineProvider
- [ ] `GoogleNotificationService` - Added ResiliencePipelineProvider
- [ ] `ServiceCollectionExtensions` - Updated all service registrations
- [ ] Build succeeds
- [ ] Tests pass
- [ ] Logs show retry attempts during failures

---

## Common Pitfalls

### ❌ DON'T: Forget to pass CancellationToken
```csharp
// WRONG
await request.ExecuteAsync();
```

### ✅ DO: Pass CancellationToken from pipeline
```csharp
// CORRECT
return await request.ExecuteAsync(cancellationToken);
```

### ❌ DON'T: Wrap entire method
```csharp
// WRONG - loses exception details
var pipeline = _resilienceProvider.GetApiCallPipeline();
await pipeline.ExecuteAsync(async ct => 
{
	// Entire method body
}, CancellationToken.None);
```

### ✅ DO: Wrap only external call
```csharp
// CORRECT - wraps only external service call
_logger.LogInformation("Starting operation");

var pipeline = _resilienceProvider.GetApiCallPipeline<TResult>();
var result = await pipeline.ExecuteAsync(async ct =>
{
	return await _externalService.CallAsync(ct);
}, CancellationToken.None);

_logger.LogInformation("Operation completed");
return result;
```

---

## Need Help?

Refer to:
- `MortgageMarketAnalysisAgent/Resilience/README.md` - Detailed guide
- `MortgageMarketAnalysisAgent/Services/Concretes/MarketAnalysisService.cs` - Working example
- `MortgageMarketAnalysisAgent/Resilience/ResiliencePipelineProvider.cs` - Pipeline definitions
