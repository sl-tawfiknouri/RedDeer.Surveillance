namespace Surveillance.Specflow.Tests.StepDefinitions.CancelledOrders
{
    public class CancelledOrderApiParameters
    {
        public decimal? CancelledOrderCountPercentageThreshold { get; set; }

        public decimal? CancelledOrderPercentagePositionThreshold { get; set; }

        public int? MaximumNumberOfTradesToApplyRuleTo { get; set; }

        public int MinimumNumberOfTradesToApplyRuleTo { get; set; }

        public int WindowHours { get; set; }
    }
}