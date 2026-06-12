using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Interfaces
{
    public interface IPromptBuilder
    {
        string BuilPrompt(HouseholdFinancialIntelligenceModel model, HousingMarketModel home, List<MarketTrend> trends);
    }
}
