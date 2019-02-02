namespace Surveillance.Specflow.Tests.StepDefinitions.HighProfit
{
    public class HighProfitApiParameters
    {
        public int WindowHours { get; set; }
        public decimal? HighProfitPercentage { get; set; }
        public decimal? HighProfitAbsolute { get; set; }
        public bool HighProfitUseCurrencyConversions { get; set; }
        public string HighProfitCurrency { get; set; }
    }
}
