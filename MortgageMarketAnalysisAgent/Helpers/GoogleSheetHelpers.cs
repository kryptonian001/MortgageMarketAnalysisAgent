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
    }
}
