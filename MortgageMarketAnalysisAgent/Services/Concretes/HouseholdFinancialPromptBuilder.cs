using Microsoft.Extensions.Logging;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class HouseholdFinancialPromptBuilder : IPromptBuilder
    {
        private readonly ILogger<HouseholdFinancialPromptBuilder> _logger;

        static readonly string TemplateRootPath = Path.Combine(AppContext.BaseDirectory, "templates");

        static readonly string ANALYSIS_REPORT_TEMPLATE_PATH = Path.Combine(TemplateRootPath, "reports", "analysis.rtl");
        static readonly string HI_FI_TEMPLATE_PATH = Path.Combine(TemplateRootPath, "prompts", "hifi.pmt");



        public HouseholdFinancialPromptBuilder(ILogger<HouseholdFinancialPromptBuilder> logger)
        {
            _logger = logger;
        }

        public string BuilPrompt(HouseholdFinancialIntelligenceModel model, HousingMarketModel home, List<MarketTrend> trends)
        {
            var prompt = new StringBuilder();

            _logger.LogInformation("Building Prompt");
            prompt.AppendLine(TemplateHelper.GetTemplate(Path.Combine(AppContext.BaseDirectory,HI_FI_TEMPLATE_PATH))
                                            .Replace("[[##ANALYSIS_DATE##]]", $"{DateTime.Today.ToString("MM/dd/yyyy")}"));

            prompt.AppendLine();

            prompt.AppendLine(model.AgentDashboard.BuildModelSummary());

            prompt.AppendLine();
            prompt.AppendLine(model.CreditProfiles.BuildCreditProfileSummary());

            prompt.AppendLine();
            prompt.AppendLine(TemplateHelper.GetTemplate(Path.Combine(AppContext.BaseDirectory, ANALYSIS_REPORT_TEMPLATE_PATH)));

            prompt.AppendLine();
            prompt.AppendLine("Here is the full household financial model as JSON:");
            prompt.AppendLine();
            prompt.AppendLine("```json");
            prompt.AppendLine(model.GetFullModelReport());
            prompt.AppendLine("```");

            prompt.AppendLine();
            prompt.AppendLine("Here is the subject house details model as JSON:");
            prompt.AppendLine();
            prompt.AppendLine("```json");
            prompt.AppendLine(home.GetHomeReport());
            prompt.AppendLine("```");

            prompt.AppendLine();
            prompt.AppendLine("Here is the housing market sales data model as JSON:");
            prompt.AppendLine();
            prompt.AppendLine("```json");
            prompt.AppendLine(trends.GetTrendReport());
            prompt.AppendLine("```");

            _logger.LogInformation("Prompt Complete");

            return prompt.ToString();
        }
    }
    
}
