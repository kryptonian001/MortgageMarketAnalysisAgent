using Newtonsoft.Json;

namespace MortgageMarketAnalysisAgent.Models.Redfin.Sold;

public class Meta
{
    public int currentPage { get; set; }
    public int limit { get; set; }
    public bool moreData { get; set; }

    [JsonProperty("message ")]
    public string message { get; set; }
}


