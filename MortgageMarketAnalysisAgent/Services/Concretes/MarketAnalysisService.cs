using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Agents.Interfaces;
using MortgageMarketAnalysisAgent.Clients;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Models.RentCast;
using MortgageMarketAnalysisAgent.Models.UsRealEstate;
using MortgageMarketAnalysisAgent.Resilience;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class MarketAnalysisService: IMarketAnalysisService
    {
        private readonly IReportBuildingService _reportBuidService;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IAgent _agent;
        private readonly INotify _notifier;
        private readonly ILogger<MarketAnalysisService> _logger;
        private readonly ResiliencePipelineProvider _resilienceProvider;

        private readonly RentCastClient _rentcastAnalyzer;
        private readonly UsRealEstateClient _usrealestateAnalyzer;

        string? emailAddress;

        public MarketAnalysisService(
            IReportBuildingService reportBuilding,
            IPromptBuilder promptBuilder,
            IAgent agent,
            INotify notifier,
            IOptions<AgentConfig> options,
            ResiliencePipelineProvider resilienceProvider,
            RentCastClient marketAnalyzer,
            UsRealEstateClient usrealestateAnalyzer,
            ILogger<MarketAnalysisService> logger) 
        {
            _reportBuidService = reportBuilding;
            _promptBuilder = promptBuilder;
            _agent = agent;
            _notifier = notifier;
            _logger = logger;
            _resilienceProvider = resilienceProvider;
            _rentcastAnalyzer = marketAnalyzer;
            _usrealestateAnalyzer = usrealestateAnalyzer;

            emailAddress = options?.Value?.NotificationEmail;
        }

        public async Task RunAnalysis()
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                _logger.LogError("❌ No email address configured. Cannot send report.");
                throw new InvalidOperationException("NotificationEmail is required in configuragtion");
            }

            List<MarketTrend> marketTrends = new List<MarketTrend>();

            var houses = await _reportBuidService.BuildMarketHouseReport();

            foreach (var house in houses)
            {
                RentCast? tentCastTrends = await _rentcastAnalyzer.AnalyzeMarket(house.PostalCode) as RentCast;

                if (tentCastTrends != null)
                {
                    var temp = RentCastClient.BuildRentCastDataSource(house, tentCastTrends);
                    marketTrends.AddRange(temp);
                }

                UsRealEstate? usRealEstateTrends = await _usrealestateAnalyzer.AnalyzeMarket(house.PostalCode) as UsRealEstate;

                if (usRealEstateTrends != null)
                {
                    var temp = UsRealEstateClient.BuildRentCastDataSource(house, usRealEstateTrends);
                    marketTrends.AddRange(temp);
                }
            }

            var pipeline = _resilienceProvider.GetApiCallPipeline();

            try
            {
                await pipeline.ExecuteAsync(async cancellationToken =>
                {
                    _logger.LogInformation("Retrieving Household_Financial_Intelligence_Agent_Ready spreadsheet");
                    var model = await _reportBuidService.BuildHouseholdFinancialIntelligenceReport();

                    _logger.LogInformation("Building market analysis prompt with Household_Financial_Intelligence_Agent_Ready infromation");
                    var prompt = _promptBuilder.BuilPrompt(model, houses[0], marketTrends);

                    _logger.LogInformation("Sending promp to ChatGPT");
                    var analysis = await _agent.RunAnalysisAsync(prompt);

                    _logger.LogInformation("Results:");
                    _logger.LogInformation(analysis);

                    _logger.LogInformation($"Sending to email: {emailAddress}");
                    await _notifier.SendEmailNotificationAsync(emailAddress, "Mortgage Refi Readiness Analysis", analysis);
                }, CancellationToken.None);
            }
            catch (Google.GoogleApiException gex)
            {
                _logger.LogError(gex, "❌ Google API error: {Message}", gex.Message);
                throw;
            }
            catch (HttpRequestException hex)
            {
                _logger.LogError(hex, "❌ Network error calling external service");
                throw;
            }
            catch (Polly.CircuitBreaker.BrokenCircuitException bcex)
            {
                _logger.LogError(bcex, "❌ Circuit breaker is open - service unavailable");
                throw;
            }
            catch (TimeoutException tex)
            {
                _logger.LogError(tex, "❌ Operation timed out");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error during analysis");
                throw;
            }
        }

        
    }
}
