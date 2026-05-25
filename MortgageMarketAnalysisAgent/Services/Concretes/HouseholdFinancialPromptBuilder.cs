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
        public string BuilPrompt(HouseholdFinancialIntelligenceModel model)
        {
            var prompt = new StringBuilder();

            prompt.AppendLine(GetAgentInstructions());

            prompt.AppendLine();

            prompt.AppendLine(BuildModelSummary(model));

            prompt.AppendLine();
            prompt.AppendLine("Here is the full household financial model as JSON:");
            prompt.AppendLine();
            prompt.AppendLine("```json");
            prompt.AppendLine(GetFullModel(model));
            prompt.AppendLine("```");

            return prompt.ToString();
        }

        private string GetFullModel(HouseholdFinancialIntelligenceModel model)
        {
            string modelJson = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return modelJson;
        }

        private string GetAgentInstructions()
        {
            var prompt = new StringBuilder();

            prompt.AppendLine("""
                You are my Household Financial Intelligence Agent.

                Your job is to analyze my household finances and produce a practical action plan.

                Primary goals:
                1. Improve mortgage refinance readiness.
                2. Preserve cash-flow safety.
                3. Optimize credit card utilization.
                4. Avoid unnecessary financial risk.
                5. Identify the best next payment allocation.

                Rules:
                - Do not recommend using more than 75% of available extra cash.
                - Keep a 25% buffer unless explicitly overridden.
                - Prioritize minimum payments first.
                - Do not recommend opening or closing credit accounts.
                - Do not assume balances are current unless they are provided.
                - If something is uncertain, mark it as uncertain.
                - If you infer something, label it with "(inferred)".
                - Do not invent missing balances, dates, payment amounts, or credit limits.
                - Treat the spreadsheet model as the source of truth for what data exists, but validate whether each value is analytically usable before relying on it.
                - Use practical, household-level recommendations, not generic financial advice.

                Data interpretation rules:
                - The spreadsheet model may contain diagnostic values, source references, formulas, or notes inside fields that look like financial fields.
                - Do not assume every field value is a usable financial amount.
                - Values like "#REF!", cell ranges, source references, empty strings, "-", and explanatory text should be treated as data quality indicators unless the field clearly represents a valid financial amount.
                - If a field contains a cell range such as "Joes!A10:Q10", treat it as a source/reference, not a financial value.
                - If a field contains explanatory text such as "Imported cash-flow line.", treat it as metadata, not a financial value.
                - If the spreadsheet provides dashboard totals, prefer those totals over recalculating from ambiguous rows.
                - If a row has ambiguous or mixed-purpose values, call that out in the data quality section instead of forcing an analysis.
                - Do not make payment recommendations from fields whose meaning is unclear.

                Report format:
                1. Executive summary
                2. Current financial position
                3. Cash-flow safety assessment
                4. Credit utilization risks
                5. Mortgage refinance readiness
                6. Recommended payment allocation
                7. What not to do
                8. Questions or missing data
                9. Next update checklist

                Output style:
                - Use clear headings.
                - Use tables where helpful.
                - Highlight action items.
                - Separate facts from assumptions.
                - Be direct and practical.
                """);

            return prompt;
        }

        private static string BuildModelSummary(HouseholdFinancialIntelligenceModel model)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Clean high-level financial summary:");
            sb.AppendLine($"- Total monthly bills: {model.AgentDashboard?.TotalMonthlyBills?.Value}");
            sb.AppendLine($"- Total credit card balance: {model.AgentDashboard?.TotalCreditCardBalance?.Value}");
            sb.AppendLine($"- Total credit card limit: {model.AgentDashboard?.TotalCreditCardLimit?.Value}");
            sb.AppendLine($"- Overall credit utilization: {model.AgentDashboard?.OverallCreditUtilization?.Value}");
            sb.AppendLine($"- Highest utilization card: {model.AgentDashboard?.HighestUtilizationCard?.Value}");
            sb.AppendLine($"- Highest utilization percentage: {model.AgentDashboard?.HightestUtilization?.Value}");
            sb.AppendLine($"- Total regular loan balance: {model.AgentDashboard?.TotalRegularLoanBalance?.Value}");
            sb.AppendLine($"- Total short-term balance: {model.AgentDashboard?.TotalShortTermBalance?.Value}");
            sb.AppendLine($"- Max extra allocation rule: {model.AgentDashboard?.MaxExtraAllocationRule?.Value}");
            sb.AppendLine($"- Open data quality issues: {model.AgentDashboard?.OpenDataQualityIssues?.Value}");

            return sb.ToString();
        }
    }
    
}
