using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MortgageMarketAnalysisAgent.Agents.Concretes;
using MortgageMarketAnalysisAgent.Agents.Interfaces;
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
        static AgentConfig googleClientCfg;
        public static async Task AddAgentConfigurationAsync(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();

            var config = builder.AddUserSecrets<Program>()
                                .AddJsonFile("appsettings.json")
                                .AddEnvironmentVariables()
                                .Build();

            var cfg = config.GetSection("AgentConfig");

            googleClientCfg = cfg.Get<AgentConfig>();

            services.Configure<AgentConfig>(cfg);

            services.AddTransient<IMarketAnalysisService, MarketAnalysisService>();

            var creds = await GetGoogleCredentials();

            services.AddTransient<GoogleDocumentService>((sp) => new GoogleDocumentService(creds));
            services.AddTransient<INotify,GoogleNotificationService>((sp) => new GoogleNotificationService(creds));

            services.AddTransient<HouseholdFinancialIntelligenceReportBuildingService>();

            services.AddTransient<IPromptBuilder, HouseholdFinancialPromptBuilder>();

            services.AddTransient<IAgent, MarketAnalysisAgent>();
        }

        private static async Task<UserCredential> GetGoogleCredentials()
        {

            UserCredential credential;

            await using (var stream = new FileStream(googleClientCfg.GoogleConfigPath, FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.FromStream(stream).Secrets;
                var scopes = new[]
                {
                    DocsService.Scope.Documents,
                    SheetsService.Scope.Spreadsheets,
                    GmailService.Scope.GmailSend
                };

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token-store", true));
            }

            return credential;
        }

    }
}
