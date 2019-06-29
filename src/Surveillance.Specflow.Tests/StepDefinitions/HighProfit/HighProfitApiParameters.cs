namespace Surveillance.Specflow.Tests.StepDefinitions.HighProfit
{
    public class HighProfitApiParameters
    {
        public int WindowHours { get; set; }
        public int FutureHours { get; set; }
        public decimal? HighProfitPercentage { get; set; }
        public decimal? HighProfitAbsolute { get; set; }
        public bool HighProfitUseCurrencyConversions { get; set; }
        public string HighProfitCurrency { get; set; }
        public bool PerformHighProfitWindowAnalysis { get; set; }
        public bool PerformHighProfitDailyAnalysis { get; set; }
    }
}
