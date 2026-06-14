namespace MortgageMarketAnalysisAgent.Models.Redfin.AutoComplete;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Datum
{
    public List<Row> rows { get; set; }
    public string name { get; set; }
}


