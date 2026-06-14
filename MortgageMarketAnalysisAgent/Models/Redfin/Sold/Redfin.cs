using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Redfin.Sold;

public class Redfin : IMarketValue
{
    public List<Datum> data { get; set; }
    public Meta meta { get; set; }
    public bool status { get; set; }
    public string message { get; set; }
}


