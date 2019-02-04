using System;

namespace Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates
{
    public class ExchangeRateApiParameters
    {
        public DateTime Date { get; set; }
        public string Name => $"{Fixed}/{Variable}";
        public string Fixed { get; set; }
        public string Variable { get; set; }
        public double Rate { get; set; }
    }
}
