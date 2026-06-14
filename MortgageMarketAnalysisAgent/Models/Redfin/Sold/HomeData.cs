namespace MortgageMarketAnalysisAgent.Models.Redfin.Sold;

public class HomeData
{
    public string propertyId { get; set; }
    public string listingId { get; set; }
    public int listingDisplayLevel { get; set; }
    public string mlsId { get; set; }
    public string url { get; set; }
    public string dataSourceId { get; set; }
    public string marketId { get; set; }
    public string mlsStatusId { get; set; }
    public string servicePolicyId { get; set; }
    public ListingMetadata listingMetadata { get; set; }
    public int propertyType { get; set; }
    public int beds { get; set; }
    public double baths { get; set; }
    public PriceInfo priceInfo { get; set; }
    public SqftInfo sqftInfo { get; set; }
    public PhotosInfo photosInfo { get; set; }
    public DaysOnMarket daysOnMarket { get; set; }
    public string timezone { get; set; }
    public YearBuilt yearBuilt { get; set; }
    public LotSize lotSize { get; set; }
    public HoaDues hoaDues { get; set; }
    public List<Sash> sashes { get; set; }
    public Brokers brokers { get; set; }
    public LastSaleData lastSaleData { get; set; }
    public Personalization personalization { get; set; }
    public Insights insights { get; set; }
    public bool showMlsId { get; set; }
    public DirectAccessInfo directAccessInfo { get; set; }
    public int fullBaths { get; set; }
    public int partialBaths { get; set; }
    public BathInfo bathInfo { get; set; }
    public bool isEarlyAccess { get; set; }
    public AddressInfo addressInfo { get; set; }
    public Photos photos { get; set; }
}


