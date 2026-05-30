using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Agents.Interfaces;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class MarketAnalysisService: IMarketAnalysisService
    {
        private readonly HouseholdFinancialIntelligenceReportBuildingService _reportBuidService;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IAgent _agent;
        private readonly INotify _notifier;

        string? emailAddress;

        public MarketAnalysisService(
            HouseholdFinancialIntelligenceReportBuildingService reportBuilding,
            IPromptBuilder promptBuilder,
            IAgent agent,
            INotify notifier,
            IOptions<AgentConfig> options) 
        {
            _reportBuidService = reportBuilding;
            _promptBuilder = promptBuilder;
            _agent = agent;
            _notifier = notifier;

            emailAddress = options?.Value.NotificationEmail;
        }

        public async Task RunAnalysis()
        {
            Console.WriteLine("Retrieving Household_Financial_Intelligence_Agent_Ready spreadsheet");
            var model = await _reportBuidService.BuildHouseholdFinancialIntelligenceReport();

            Console.WriteLine("Building market analysis prompt with Household_Financial_Intelligence_Agent_Ready infromation");
            var prompt = _promptBuilder.BuilPrompt(model);

            Console.WriteLine("Sending promp to ChatGPT");
            var analysis = await _agent.RunAnalysisAsync(prompt);

            Console.WriteLine("Results:");
            Console.WriteLine(analysis);

            Console.WriteLine($"Sending to email: {emailAddress}");
            await _notifier.SendEmailNotificationAsync(emailAddress, "Mortgage Refi Readiness Analysis", analysis);
        }
    }
}
