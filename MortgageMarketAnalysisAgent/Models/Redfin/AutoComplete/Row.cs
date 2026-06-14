namespace MortgageMarketAnalysisAgent.Models.Redfin.AutoComplete;

public class Row
{
    public string id { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public string subName { get; set; }
    public string url { get; set; }
    public bool active { get; set; }
    public bool claimedHome { get; set; }
    public bool invalidMRS { get; set; }
    public List<int> businessMarketIds { get; set; }
    public string countryCode { get; set; }
    public int internalSearchVolume { get; set; }
}


