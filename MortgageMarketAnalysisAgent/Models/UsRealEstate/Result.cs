namespace MortgageMarketAnalysisAgent.Models.UsRealEstate;

public class Result
{
    public int bathrooms { get; set; }
    public int bedrooms { get; set; }
    public string city { get; set; }
    public string country { get; set; }
    public string currency { get; set; }
    public object datePriceChanged { get; set; }
    public int daysOnZillow { get; set; }
    public string homeStatus { get; set; }
    public string homeStatusForHDP { get; set; }
    public string homeType { get; set; }
    public string imgSrc { get; set; }
    public bool isFeatured { get; set; }
    public bool isNonOwnerOccupied { get; set; }
    public bool isPreforeclosureAuction { get; set; }
    public bool isPremierBuilder { get; set; }
    public bool isShowcaseListing { get; set; }
    public bool isUnmappable { get; set; }
    public bool isZillowOwned { get; set; }
    public double latitude { get; set; }
    public ListingSubType listing_sub_type { get; set; }
    public int livingArea { get; set; }
    public double longitude { get; set; }
    public string lotAreaUnit { get; set; }
    public double lotAreaValue { get; set; }
    public int price { get; set; }
    public int priceChange { get; set; }
    public int priceForHDP { get; set; }
    public string priceReduction { get; set; }
    public bool shouldHighlight { get; set; }
    public string state { get; set; }
    public string streetAddress { get; set; }
    public int taxAssessedValue { get; set; }
    public Int64 timeOnZillow { get; set; }
    public string zipcode { get; set; }
    public int zpid { get; set; }
    public int? rentZestimate { get; set; }
    public int? zestimate { get; set; }
    public string newConstructionType { get; set; }
    public string providerListingID { get; set; }
    public string group_type { get; set; }
    public string priceSuffix { get; set; }
    public string unit { get; set; }
    public string openHouse { get; set; }
    public OpenHouseInfo open_house_info { get; set; }
}
