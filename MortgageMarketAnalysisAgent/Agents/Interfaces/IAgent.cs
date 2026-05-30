namespace MortgageMarketAnalysisAgent.Agents.Interfaces
{
    public interface IAgent
    {
        Task<string> RunAnalysisAsync(string mortgageReadiness);
    }
}