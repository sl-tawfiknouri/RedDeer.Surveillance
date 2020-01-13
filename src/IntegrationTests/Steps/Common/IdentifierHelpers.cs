using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Steps.Common
{
    public static class IdentifierHelpers
    {
        public static string ToIsinOrFigi(string value)
        {
            var isin = value;
            while (isin.Length < 12)
            {
                isin += "x";
            }
            return isin;
        }
    }
}
