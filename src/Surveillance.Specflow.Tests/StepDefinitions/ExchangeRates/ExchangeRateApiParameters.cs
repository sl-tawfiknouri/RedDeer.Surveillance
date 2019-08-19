namespace Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates
{
    using System;

    public class ExchangeRateApiParameters
    {
        public DateTime Date { get; set; }

        public string Fixed { get; set; }

        public string Name => $"{this.Fixed}/{this.Variable}";

        public double Rate { get; set; }

        public string Variable { get; set; }
    }
}