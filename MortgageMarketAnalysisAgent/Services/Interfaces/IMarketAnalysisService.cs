using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Interfaces
{
    public interface IMarketAnalysisService
    {
        Task RunAnalysis();
    }
}
