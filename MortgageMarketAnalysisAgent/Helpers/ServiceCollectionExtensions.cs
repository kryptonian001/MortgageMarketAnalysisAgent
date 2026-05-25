using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Services.Concretes;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static async Task AddAgentConfigurationAsync(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();

            var config = builder.AddUserSecrets<Program>()
                                .AddEnvironmentVariables()
                                .Build();

            var githubToken = config["GitHub:Token"]
                ?? throw new InvalidOperationException("Missing GitHub token.");

            
            var openAiKey = config["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("Missing OpenAI API key.");

            AgentConfig agentCfg = new AgentConfig();
            config.Bind(agentCfg);
            services.AddOptions<AgentConfig>();

            services.AddTransient<IMarketAnalysisService, MarketAnalysisService>();

            var creds = await GetGoogleCredentials();

            services.AddTransient<GoogleDocumentService>((sp) => new GoogleDocumentService(creds));

            services.AddTransient<HouseholdFinancialIntelligenceReportBuildingService>();

            services.AddTransient<IPromptBuilder, HouseholdFinancialPromptBuilder>();
        }

        private static async Task<UserCredential> GetGoogleCredentials()
        {

            UserCredential credential;

            await using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    new[] { DocsService.Scope.Documents, SheetsService.Scope.Spreadsheets },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token-store", true));
            }

            return credential;
        }

    }
}
