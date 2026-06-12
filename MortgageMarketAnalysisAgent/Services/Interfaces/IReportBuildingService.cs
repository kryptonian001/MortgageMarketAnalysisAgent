using MortgageMarketAnalysisAgent.Models.Documents;

namespace MortgageMarketAnalysisAgent.Services.Interfaces
{
    public interface IReportBuildingService
    {
        Task<HouseholdFinancialIntelligenceModel> BuildHouseholdFinancialIntelligenceReport();
        Task<List<HousingMarketModel>> BuildMarketHouseReport();
    }
}