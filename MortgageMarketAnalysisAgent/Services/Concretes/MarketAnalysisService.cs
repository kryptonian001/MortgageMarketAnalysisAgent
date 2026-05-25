using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class MarketAnalysisService: IMarketAnalysisrService
    {
        private readonly ReportBuildingService _reportBuidService;


        public MarketAnalysisService(ReportBuildingService reportBuilding) 
        {
            _reportBuidService = reportBuilding;
        }

        public async Task RunAnalysis()
        {
            var reoprt = await _reportBuidService.BuildHouseholdFinancialIntelligenceReport();
        }



    }
}
