using MortgageMarketAnalysisAgent.Models.Documents.Components;
using OpenAI.Graders;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MortgageMarketAnalysisAgent.Helpers
{
    public static class GoogleSheetHelpers
    {
        public static string SafeString(this IList<object> row, int index)
        {
            try
            {
                return row[index].ToString();
            }
            catch (ArgumentOutOfRangeException)
            {
                return "";
            }
        }


        public static int? CalculateMortgageMiddleScore(this CreditProfile profile)
        {
            var scores = new[]
            {
                TryParseScore(profile.Fico5),
                TryParseScore(profile.Fico4),
                TryParseScore(profile.Fico2)
            }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .OrderBy(x => x)
            .ToList();

            if (scores.Count != 3)
            {
                return null;
            }

            return scores[1];
        }

        private static int? TryParseScore(string value)
        {
            if (int.TryParse(value, out int score))
            {
                return score;
            }

            return null;
        }
    }
}
