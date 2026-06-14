using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Models.Redfin.AutoComplete;

public class Locale
{
    public List<Datum> data { get; set; }
    public bool status { get; set; }
    public string message { get; set; }

    public List<PropertySearchLocales>? ZipCodeIds =>
        data?.Where(p => p.name.Equals("places", StringComparison.OrdinalIgnoreCase))
        .SelectMany(p => p.rows)
        .Select(p => new PropertySearchLocales(p.id, p.name, p.subName))
        .ToList();
        
}

public record struct PropertySearchLocales(string Id, string Name, string Subname);


