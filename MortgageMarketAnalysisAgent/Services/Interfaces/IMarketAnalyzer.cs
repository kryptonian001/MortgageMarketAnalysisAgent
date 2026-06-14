using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.RentCast;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Interfaces;

public interface IMarketAnalyzer
{
    Task<IMarketValue> AnalyzeMarket(string zipcode);
    Task<IMarketValue> AnalyzeMarket(HousingMarketModel house);
}
