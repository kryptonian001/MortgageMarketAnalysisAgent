using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Documents.Components;

public class MarketTrend
{

    public string Source { get; set; }
    public string ZipCode { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<PropertyTypeMarketData> PropertyTypeMarketData { get; set; } = new();
    public List<BedroomMarketData> BedroomMarketData { get; set; } = new();
}

public class PropertyTypeMarketData
{
    public string PropertyType { get; set; }
    public double MedianPrice { get; set; }
    public double MedianPricePerSquareFoot { get; set; }
    public int MedianSquareFootage { get; set; }
    public int TotalListings { get; set; }
    public int MedianDaysOnMarket { get; set; }
}

public class BedroomMarketData
{
    public string Role { get; set; }
    public string? AnchorDirection { get; set; }
    public string PropertyType { get; set; }
    public int PropertyTypeMedian { get; set; }
    public double? propertyTypeMedianPricePerSquareFoot { get; set; }
    public int BedRooms { get; set; }
    public double BathRooms { get; set; }
    public bool BathRoomsIsMarketFilter { get; set; }
    public double MedianPrice { get; set; }
    public double MedianPricePerSquareFoot { get; set; }
    public int MedianSquareFootage { get; set; }
    public int TotalListings { get; set; }
    public int MedianDaysOnMarket { get; set; }
}