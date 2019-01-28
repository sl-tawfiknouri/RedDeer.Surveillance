namespace Surveillance.Specflow.Tests.StepDefinitions.WashTrades
{
    public class WashTradeApiParameters
    {
        public int WindowHours { get; set; }
        public int? MinimumNumberOfTrades { get; set; }
        public decimal? MaximumPositionChangeValue { get; set; }
        public int? MaximumAbsoluteValueChange { get; set; }
        public string MaximumAbsoluteValueChangeCurrency { get; set; }
    }
}
