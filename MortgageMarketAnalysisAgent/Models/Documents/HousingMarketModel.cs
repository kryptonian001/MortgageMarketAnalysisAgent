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
    public string PropertyType { get; set; }
    public string YearBuilt { get; set; }
    public int Stories { get; set; }
    public string Garage { get; set; }
    public int LotSize { get; set; }
    public string HOA { get; set; }
    public string HoaAnnualFee { get; set; }
    public string AppraisedValue { get; set; }
    public string AppraisalDate { get; set; }
    public string PostAppraisalUpgrades { get; set; }
    public string FutureProjectedUpgrades { get; set; }
}

