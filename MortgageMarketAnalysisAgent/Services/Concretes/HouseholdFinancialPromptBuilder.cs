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
            prompt.AppendLine(BuildCreditProfileSummary(model));

            prompt.AppendLine();
            prompt.AppendLine(GetReportTemplate());

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

                Credit profile interpretation rules:
                - Use the CreditProfiles section for refinance credit readiness.
                - For mortgage readiness, focus on FICO 5, FICO 4, and FICO 2.
                - Calculate each person's mortgage middle score as the middle value of FICO 5, FICO 4, and FICO 2.
                - If both borrowers are included, use the lower mortgage middle score as the conservative household planning score.
                - Do not use Vantage 3.0 as the mortgage readiness score.
                - Consider score date and data confidence when assessing reliability.
                - If scores are older than 30 days, mark the score data as stale or needing refresh.

                Mortgage refinance readiness interpretation:
                - In mortgageRefiReadinesses, a field is NOT missing if the "value" field contains a non-empty, non-placeholder value.
                - Do not treat status = "Needed" as meaning the value is missing when a value is present.
                - If "value" is present but "status" says "Needed", classify it as "provided but status may be stale or contradictory."
                - Only call a refinance input missing when value is empty, "-", "#REF!", "N/A", null, or clearly not usable.
                - In the executive summary, do not say mortgage balances, rates, escrow, or term are missing if those values are present in mortgageRefiReadinesses.

                Consistency rules:
                - Always use the provided report template exactly.
                - Do not rename sections.
                - Do not reorder sections.
                - Do not omit sections.
                - Do not create new top-level sections.
                - Keep table columns exactly as provided.
                - If a value is unavailable, use "Not provided".
                - If a value exists but conflicts with status/source metadata, use "Provided but metadata conflicts".
                - Do not summarize away required tables.
                - Return markdown only.
                - Do not wrap the report in a code block.

                Report structure:
                - Use the provided markdown report template exactly.
                - Do not add, remove, rename, or reorder sections.
                - Do not change table column names.
                - Fill every section.
                - If a value is unavailable, use "Not provided".
                - If a value exists but conflicts with status/source metadata, write "Provided but metadata conflicts".
                - Return markdown only.
                - Do not wrap the report in a code block.

                Output style:
                - Use clear headings.
                - Use tables where helpful.
                - Highlight action items.
                - Separate facts from assumptions.
                - Be direct and practical.
                """);

            return prompt.ToString();
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

        private static string BuildCreditProfileSummary(HouseholdFinancialIntelligenceModel model)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Clean credit profile summary:");

            foreach (var profile in model.CreditProfiles)
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

            int? planningScore = model.CreditProfiles
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

        private static string GetReportTemplate()
        {
            return """
                Report template:
                Use the following exact markdown structure.
                Do not add, remove, rename, or reorder sections.
                Fill every section.
                If a section has no usable data, write "Not provided" or "Uncertain" and explain why.

                # Household Financial Intelligence Report

                ## 1. Executive Summary

                | Area | Status | Key Takeaway |
                |---|---|---|
                | Cash Flow |  |  |
                | Credit Utilization |  |  |
                | Mortgage Refinance Readiness |  |  |
                | Data Quality |  |  |
                | Recommended Next Action |  |  |

                Summary:
                - 
                - 
                - 

                ## 2. Current Financial Position

                | Metric | Value | Notes |
                |---|---:|---|
                | Total Monthly Bills |  |  |
                | Total Credit Card Balance |  |  |
                | Total Credit Card Limit |  |  |
                | Overall Credit Utilization |  |  |
                | Total Regular Loan Balance |  |  |
                | Total Short-Term Balance |  |  |

                ## 3. Cash-Flow Safety Assessment

                | Pay Date / Period | Income | Required Expenses | After Expenses | 25% Buffer | Max Usable Extra |
                |---|---:|---:|---:|---:|---:|
                |  |  |  |  |  |  |

                Cash-flow assessment:
                - 
                - 

                ## 4. Credit Utilization Risks

                | Card | Balance | Limit | Utilization | Risk Level | Recommended Action |
                |---|---:|---:|---:|---|---|
                |  |  |  |  |  |  |

                Utilization priority:
                1. 
                2. 
                3. 

                ## 5. Credit Profile and Mortgage Score Readiness

                | Person | FICO 5 | FICO 4 | FICO 2 | Mortgage Middle Score | Score Date | Confidence | Notes |
                |---|---:|---:|---:|---:|---|---|---|
                |  |  |  |  |  |  |  |  |

                Mortgage score readiness:
                - Conservative household planning score: 
                - Target mortgage score: 
                - Readiness status: 

                ## 6. Mortgage Refinance Readiness

                | Input | Value | Status Interpretation | Notes |
                |---|---:|---|---|
                | First Mortgage Balance |  |  |  |
                | First Mortgage Rate |  |  |  |
                | Term Remaining |  |  |  |
                | P&I Payment |  |  |  |
                | Escrow |  |  |  |
                | Second Mortgage Balance |  |  |  |
                | Second Mortgage Payment |  |  |  |
                | Second Mortgage Rate |  |  |  |
                | Target Refinance Month |  |  |  |
                | Target Minimum Mortgage FICO |  |  |  |

                Refinance readiness assessment:
                - 
                - 

                ## 7. Recommended Payment Allocation

                | Priority | Target | Recommended Amount | Reason |
                |---:|---|---:|---|
                | 1 |  |  |  |
                | 2 |  |  |  |
                | 3 |  |  |  |

                Allocation rules applied:
                - Minimum payments first.
                - No more than 75% of positive after-expense cash.
                - Preserve 25% buffer.

                ## 8. What Not To Do

                - 
                - 
                - 

                ## 9. Questions or Missing Data

                | Item | Why It Matters | Impact |
                |---|---|---|
                |  |  |  |

                ## 10. Next Update Checklist

                - [ ] 
                - [ ] 
                - [ ] 

                ## 11. Data Quality Notes

                | Issue | Location / Field | Recommended Fix |
                |---|---|---|
                |  |  |  |
                """;
        }
    }
    
}
