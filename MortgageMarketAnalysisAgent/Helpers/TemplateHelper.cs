using MortgageMarketAnalysisAgent.Models.Documents;
using MortgageMarketAnalysisAgent.Models.Documents.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Helpers
{
    public static class TemplateHelper
    {
        public static string GetFullModelReport(this HouseholdFinancialIntelligenceModel model)
        {
            string modelJson = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return modelJson;
        }

        public static string GetTemplate(string path)
        {
            string? templateText = File.ReadAllText(path);

            return templateText;
        }

        public static string BuildModelSummary(this AgentDashboard dashboard)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Clean high-level financial summary:");
            sb.AppendLine($"- Total monthly bills: {dashboard.TotalMonthlyBills?.Value}");
            sb.AppendLine($"- Total credit card balance: {dashboard.TotalCreditCardBalance?.Value}");
            sb.AppendLine($"- Total credit card limit: {dashboard.TotalCreditCardLimit?.Value}");
            sb.AppendLine($"- Overall credit utilization: {dashboard.OverallCreditUtilization?.Value}");
            sb.AppendLine($"- Highest utilization card: {dashboard.HighestUtilizationCard?.Value}");
            sb.AppendLine($"- Highest utilization percentage: {dashboard.HightestUtilization?.Value}");
            sb.AppendLine($"- Total regular loan balance: {dashboard.TotalRegularLoanBalance?.Value}");
            sb.AppendLine($"- Total short-term balance: {dashboard.TotalShortTermBalance?.Value}");
            sb.AppendLine($"- Max extra allocation rule: {dashboard.MaxExtraAllocationRule?.Value}");
            sb.AppendLine($"- Open data quality issues: {dashboard.OpenDataQualityIssues?.Value}");

            return sb.ToString();
        }

        public static string BuildCreditProfileSummary(this List<CreditProfile> profiles)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Clean credit profile summary:");

            foreach (var profile in profiles)
            {
                sb.AppendLine(
                    $"- {profile.Person}: FICO 8={profile.Fico8}, " +
                    $"FICO 5={profile.Fico5}, FICO 4={profile.Fico4}, FICO 2={profile.Fico2}, " +
                    $"Mortgage middle score={profile.MortgageMiddleScore}, " +
                    $"Vantage 3.0={profile.Vantage3_0}, " +
                    $"Score date={profile.ScoreDate}, " +
                    $"Data confidence={profile.DataConfidence}, " +
                    $"Notes={profile.Notes}");
            }

            int? planningScore = profiles
                .Select(x => x.MortgageMiddleScore)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .DefaultIfEmpty()
                .Min();

            if (planningScore > 0)
            {
                sb.AppendLine($"- Conservative household planning score: {planningScore}");
            }

            return sb.ToString();
        }
    }
}
