using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace MortgageMarketAnalysisAgent.Resilience
{
    /// <summary>
    /// Provides resilience pipelines for different operation types
    /// </summary>
    public class ResiliencePipelineProvider
    {
        private readonly ILogger<ResiliencePipelineProvider> _logger;

        public ResiliencePipelineProvider(ILogger<ResiliencePipelineProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Pipeline for external API calls (Google Sheets, Docs, OpenAI)
        /// Includes: Timeout -> Retry -> Circuit Breaker
        /// </summary>
        public ResiliencePipeline<TResult> GetApiCallPipeline<TResult>()
        {
            return new ResiliencePipelineBuilder<TResult>()
                .AddTimeout(TimeSpan.FromSeconds(30))
                .AddRetry(new RetryStrategyOptions<TResult>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        _logger.LogWarning(
                            "Retry attempt {AttemptNumber} after {Delay}ms due to: {Exception}",
                            args.AttemptNumber,
                            args.RetryDelay.TotalMilliseconds,
                            args.Outcome.Exception?.Message ?? "Unknown error");
                        return ValueTask.CompletedTask;
                    },
                    ShouldHandle = new PredicateBuilder<TResult>()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>()
                        .Handle<TimeoutException>()
                        .Handle<Google.GoogleApiException>(ex => 
                            ex.HttpStatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                            ex.HttpStatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                            ex.HttpStatusCode == System.Net.HttpStatusCode.GatewayTimeout)
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions<TResult>
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    OnOpened = args =>
                    {
                        _logger.LogError(
                            "Circuit breaker opened due to: {Exception}",
                            args.Outcome.Exception?.Message ?? "Failure threshold exceeded");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        _logger.LogInformation("Circuit breaker closed, normal operation resumed");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        _logger.LogInformation("Circuit breaker half-opened, testing service availability");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }

        /// <summary>
        /// Pipeline for critical operations that should not be retried
        /// Includes: Timeout only
        /// </summary>
        public ResiliencePipeline<TResult> GetCriticalOperationPipeline<TResult>()
        {
            return new ResiliencePipelineBuilder<TResult>()
                .AddTimeout(TimeSpan.FromSeconds(60))
                .Build();
        }

        /// <summary>
        /// Pipeline for notification operations
        /// Includes: Timeout -> Retry (fewer attempts)
        /// </summary>
        public ResiliencePipeline<TResult> GetNotificationPipeline<TResult>()
        {
            return new ResiliencePipelineBuilder<TResult>()
                .AddTimeout(TimeSpan.FromSeconds(15))
                .AddRetry(new RetryStrategyOptions<TResult>
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        _logger.LogWarning(
                            "Notification retry {AttemptNumber} due to: {Exception}",
                            args.AttemptNumber,
                            args.Outcome.Exception?.Message ?? "Unknown error");
                        return ValueTask.CompletedTask;
                    },
                    ShouldHandle = new PredicateBuilder<TResult>()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>()
                        .Handle<TimeoutException>()
                })
                .Build();
        }

        /// <summary>
        /// Non-generic pipeline for void operations
        /// </summary>
        public ResiliencePipeline GetApiCallPipeline()
        {
            return new ResiliencePipelineBuilder()
                .AddTimeout(TimeSpan.FromSeconds(30))
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        _logger.LogWarning(
                            "Retry attempt {AttemptNumber} after {Delay}ms due to: {Exception}",
                            args.AttemptNumber,
                            args.RetryDelay.TotalMilliseconds,
                            args.Outcome.Exception?.Message ?? "Unknown error");
                        return ValueTask.CompletedTask;
                    },
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>()
                        .Handle<TimeoutException>()
                        .Handle<Google.GoogleApiException>(ex =>
                            ex.HttpStatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                            ex.HttpStatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                            ex.HttpStatusCode == System.Net.HttpStatusCode.GatewayTimeout)
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    OnOpened = args =>
                    {
                        _logger.LogError(
                            "Circuit breaker opened due to: {Exception}",
                            args.Outcome.Exception?.Message ?? "Failure threshold exceeded");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        _logger.LogInformation("Circuit breaker closed, normal operation resumed");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        _logger.LogInformation("Circuit breaker half-opened, testing service availability");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }
    }
}
