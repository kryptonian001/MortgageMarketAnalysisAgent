using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.UsRealEstate;

public class UsRealEstate : IMarketValue
{
    public List<Result> results { get; set; }
    public int resultsPerPage { get; set; }
    public int totalPages { get; set; }
    public int totalResultCount { get; set; }
}
