using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Steps.Common
{
    public static class RuleParameterHelpers
    {
        public static TimeSpan ToWindowSize(string timeWindow)
        {
            if (timeWindow == null)
            {
                return TimeSpan.Zero;
            }

            var split = timeWindow.Split(" ");
            if (split.Length == 2)
            {
                var value = split[0];
                var type = split[1];
                if (type == "day" || type == "days")
                {
                    return TimeSpan.FromDays(Convert.ToInt32(split[0]));
                }
            }

            return TimeSpan.Zero;
        }
    }
}
