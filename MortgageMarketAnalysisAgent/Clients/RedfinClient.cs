using Google.Apis.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Models.Redfin.AutoComplete;
using MortgageMarketAnalysisAgent.Models.Redfin.Sold;
using MortgageMarketAnalysisAgent.Models.RentCast;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using Newtonsoft.Json;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace MortgageMarketAnalysisAgent.Clients;

public class RedfinClient : IMarketAnalyzer
{
    private readonly HttpClient client;
    private readonly RedfinConfig _config;
    

    private const string x_rapidapi_key = "98f6ec649bmsh2e09cbfdfea7e9bp19e63ajsn873b8410ec7f";
    private const string x_rapidapi_host = "redfin-com-data.p.rapidapi.com";

    private readonly IReportBuildingService _reportBuildingService;

    const string TARGET_PROPERTY_TYPE = "Single Family";
    const string SUBJECT_ROLE = "SubjectBedroomBucket";
    const string ADJACENT_ROLE = "AdjacentBedroomAnchor";
    const string ANCHOR_NEXT_HIGHER = "NextHigherBedroomBucket";


    public RedfinClient(HttpClient httpClient, IReportBuildingService reportBuildingService, IOptions<RapidApiConfig> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value);

        client = httpClient;
        _config = options.Value.RedfinConfig;

        _reportBuildingService = reportBuildingService;
    }

    public async Task<IMarketValue?> AnalyzeMarket(HousingMarketModel house)
    {
        var locale = await GetZipcodeId(house.City, house.State);
        var propertyLocale = locale.ZipCodeIds?
                              .FirstOrDefault(p => p.Subname.Contains(house.City));

        if (propertyLocale == null)
            return null;

        var zipCodeId = propertyLocale.Value.Id;

        var respoonse = await GetPropertyDetails(house, zipCodeId);

        return respoonse;
    }

    
    public async Task<IMarketValue> AnalyzeMarket(string zipcode)
    {
        var locale = await GetZipcodeId(zipcode);
        var propertyLocale = locale.ZipCodeIds?
                              .FirstOrDefault(p => p.Name.Equals(zipcode));

        if (propertyLocale == null)
            return null;

        var zipCodeId = propertyLocale.Value.Id;

        var respoonse = await GetPropertyDetails(zipCodeId);

        return respoonse;
    }

    private async Task<Locale> GetZipcodeId(string state="", string city="")
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://redfin-com-data.p.rapidapi.com/properties/auto-complete?query={city} {state}"),
            Headers =
            {
                { "x-rapidapi-key", _config.ApiKey },
                { "x-rapidapi-host", _config.Host },
            },
        };

        return await HttpRequest<Locale>(request);
    }

    private async Task<Redfin> GetPropertyDetails(string zipCodeId)
    {
        var uri = new Uri($"https://redfin-com-data.p.rapidapi.com/properties/search-sold?regionId={zipCodeId}&soldWithin=30");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri,
            Headers =
            {
                { "x-rapidapi-key", _config.ApiKey },
                { "x-rapidapi-host", _config.Host},
            },
        };

        return await HttpRequest<Redfin>(request);
    }

    private async Task<Redfin> GetPropertyDetails(HousingMarketModel house, string zipCodeId)
    {
        var uri = new Uri($"https://redfin-com-data.p.rapidapi.com/properties/search-sold?regionId={zipCodeId}&soldWithin=30&beds={house.BedRooms}&baths={house.BathRooms}&homeType=1&squareFeet={GetSquareFootageRange(house.SquareFootage, 500)}&lotSize={GetSquareFootageRange(house.LotSize, 500)}");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri,
            Headers =
            {
                { "x-rapidapi-key", _config.ApiKey },
                { "x-rapidapi-host", _config.Host },
            },
        };

        return await HttpRequest<Redfin>(request);
    }


    private string GetSquareFootageRange(double value, double spread)
    {
        return $"{value - spread}, {value + spread}";
    }

    private string GetSquareFootageRange(int value, int spread)
    {
        return $"{value - spread}, {value + spread}";
    }

    private async Task<T> HttpRequest<T>(HttpRequestMessage request)
    {
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body) ?? default!;
        }
    }

    public static List<MarketTrend> BuildRedfinDataSource(HousingMarketModel house, Redfin trends)
    {
        List<MarketTrend> marketTrends = new List<MarketTrend>();

        var bedroomData = trends.data
                                    .Where(p =>
                                              (p.homeData.beds.Equals(house.BedRooms) &&
                                              p.homeData.sqftInfo.amount.ToSafeInt() <= house.SquareFootage &&
                                              p.homeData.sqftInfo.amount.ToSafeInt() >= house.SquareFootage ||
                                              p.homeData.beds.Equals(house.BedRooms + 1)))
                                       .ToList();

        var propTypData = new
        {
            MedianPrice = (int)trends.data.Average(p => p.homeData.priceInfo.amount.ToSafeInt()),
            MedianSquareFt = trends.data.Average(p => p.homeData.sqftInfo.amount.ToSafeDouble()),
        };


        marketTrends.Add(new MarketTrend
        {
            Source = nameof(RentCast),
            ZipCode = house.PostalCode,
            LastUpdated = DateTime.Today,//bedroomData.Max(p => p.homeData.lastSaleData).lastSoldDate,
            PropertyTypeMarketData = bedroomData.Where(p => p.homeData.beds.Equals(house.BedRooms))
                                                .Select(p => new PropertyTypeMarketData
                                                {
                                                    PropertyType = TARGET_PROPERTY_TYPE,
                                                    MedianPrice = p.homeData.priceInfo.amount.ToSafeDouble(),
                                                    MedianPricePerSquareFoot = p.homeData.sqftInfo.amount.ToSafeInt(),
                                                    MedianSquareFootage = p.homeData.sqftInfo.amount.ToSafeInt(),
                                                    TotalListings = bedroomData.Count(),
                                                    MedianDaysOnMarket = 0
                                                }).ToList(),
            BedroomMarketData = bedroomData.Select(p => new BedroomMarketData
            {
                Role = p.homeData.beds == house.BedRooms ? SUBJECT_ROLE : ADJACENT_ROLE,
                AnchorDirection = p.homeData.beds > house.BedRooms ? ANCHOR_NEXT_HIGHER : null,
                BedRooms = p.homeData.beds,
                BathRooms = house.BathRooms,
                MedianPrice = p.homeData.priceInfo.amount.ToSafeDouble(),
                MedianPricePerSquareFoot = p.homeData.sqftInfo.amount.ToSafeInt(),
                MedianSquareFootage = p.homeData.sqftInfo.amount.ToSafeInt(),
                TotalListings = bedroomData.Count(x => x.homeData.beds.Equals(p.homeData.beds)),
                MedianDaysOnMarket = 0,
                PropertyType = TARGET_PROPERTY_TYPE,
                PropertyTypeMedian = propTypData.MedianPrice,
                propertyTypeMedianPricePerSquareFoot = propTypData.MedianSquareFt
            }).ToList()
        });

        return marketTrends;
    }

}
