using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Services.Concretes;
using MortgageMarketAnalysisAgent.Services.Interfaces;


namespace MortgageMarketAnalysisAgent
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = Host.CreateApplicationBuilder(args);

                await builder.Services.AddAgentConfigurationAsync();

                builder.Logging.AddConsole();

                using var host = builder.Build();

                var scRunner = host.Services.GetRequiredService<IMarketAnalysisService>();

                await scRunner.RunAnalysis();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}
