namespace MortgageMarketAnalysisAgent.Models.RentCast;

public class DataByPropertyType
{
    public string propertyType { get; set; }
    public int averagePrice { get; set; }
    public int medianPrice { get; set; }
    public int minPrice { get; set; }
    public int maxPrice { get; set; }
    public double? averagePricePerSquareFoot { get; set; }
    public double? medianPricePerSquareFoot { get; set; }
    public double? minPricePerSquareFoot { get; set; }
    public double? maxPricePerSquareFoot { get; set; }
    public int? averageSquareFootage { get; set; }
    public int? medianSquareFootage { get; set; }
    public int? minSquareFootage { get; set; }
    public int? maxSquareFootage { get; set; }
    public double averageDaysOnMarket { get; set; }
    public int medianDaysOnMarket { get; set; }
    public int minDaysOnMarket { get; set; }
    public int maxDaysOnMarket { get; set; }
    public int newListings { get; set; }
    public int totalListings { get; set; }
}
