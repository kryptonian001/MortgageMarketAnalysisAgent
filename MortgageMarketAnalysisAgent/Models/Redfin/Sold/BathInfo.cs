namespace MortgageMarketAnalysisAgent.Models.Redfin.Sold;

public class BathInfo
{
    public int rawHalfBaths { get; set; }
    public int rawFullBaths { get; set; }
    public int computedPartialBaths { get; set; }
    public int computedFullBaths { get; set; }
    public double computedTotalBaths { get; set; }
}


