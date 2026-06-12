using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.RentCast;

public class RentCast : IMarketValue
{
    public string id { get; set; }
    public string zipCode { get; set; }
    public SaleData saleData { get; set; }
}
