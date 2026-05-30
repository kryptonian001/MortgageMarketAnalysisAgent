using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    internal class HouseholdFinancialPromptBuilder : IPromptBuilder
    {
        const string ANALYSIS_REPORT_TEMPLATE_PATH = @"templates\reports\analysis.rtl";

        const string HI_FI_TEMPLATE_PATH = @"templates\prompts\hifi.pmt";

        public string BuilPrompt(HouseholdFinancialIntelligenceModel model)
        {
            var prompt = new StringBuilder();

            prompt.AppendLine(TemplateHelper.GetTemplate(HI_FI_TEMPLATE_PATH)
                                            .Replace("[[##ANALYSIS_DATE##]]", $"{DateTime.Today.ToString("MM/dd/yyyy")}"));

            prompt.AppendLine();

            prompt.AppendLine(model.AgentDashboard.BuildModelSummary());

            prompt.AppendLine();
            prompt.AppendLine(model.CreditProfiles.BuildCreditProfileSummary());

            prompt.AppendLine();
            prompt.AppendLine(TemplateHelper.GetTemplate(ANALYSIS_REPORT_TEMPLATE_PATH));

            prompt.AppendLine();
            prompt.AppendLine("Here is the full household financial model as JSON:");
            prompt.AppendLine();
            prompt.AppendLine("```json");
            prompt.AppendLine(model.GetFullModelReport());
            prompt.AppendLine("```");

            return prompt.ToString();
        }
    }
    
}
