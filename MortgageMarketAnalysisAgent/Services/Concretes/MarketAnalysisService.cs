using MortgageMarketAnalysisAgent.Helpers;
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


        public MarketAnalysisService(HouseholdFinancialIntelligenceReportBuildingService reportBuilding, IPromptBuilder promptBuilder) 
        {
            _reportBuidService = reportBuilding;
            _promptBuilder = promptBuilder;
        }

        public async Task RunAnalysis()
        {
            var model = await _reportBuidService.BuildHouseholdFinancialIntelligenceReport();

            var prompt = _promptBuilder.BuilPrompt(model);
        }



    }
}
