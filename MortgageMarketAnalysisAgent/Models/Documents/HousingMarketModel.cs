using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Documents;

public class HousingMarketModel
{
    public string StreetAddress { get; set; }
    public string PostalCode { get; set; }
    public int BedRooms { get; set; }
    public double BathRooms { get; set; }
    public double SquareFootage { get; set; }
}

