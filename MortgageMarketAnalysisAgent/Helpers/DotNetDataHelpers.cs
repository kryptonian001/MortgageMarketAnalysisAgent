using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Helpers;

public static class DotNetDataHelpers
{

    public static int ToSafeInt(this string value)
    {
        if (!int.TryParse(value, out int number))
            return 0;

        return number;
    }

    public static double ToSafeDouble(this string value)
    {
        if (!double.TryParse(value, out double number))
            return 0.0;

        return number;
    }
}
