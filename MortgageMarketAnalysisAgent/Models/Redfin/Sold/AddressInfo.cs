namespace MortgageMarketAnalysisAgent.Models.Redfin.Sold;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AddressInfo
{
    public Centroid centroid { get; set; }
    public string formattedStreetLine { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
    public string location { get; set; }
    public int streetlineDisplayLevel { get; set; }
    public int unitNumberDisplayLevel { get; set; }
    public int locationDisplayLevel { get; set; }
    public int countryCode { get; set; }
    public int postalCodeDisplayLevel { get; set; }
}


