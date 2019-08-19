namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface ICancelledOrderRuleBreach : IRuleBreach
    {
        int? AmountOfPositionCancelled { get; }

        int? AmountOfPositionInTotal { get; }

        bool ExceededPercentagePositionCancellations { get; }

        bool ExceededPercentageTradeCountCancellations { get; }

        ICancelledOrderRuleEquitiesParameters Parameters { get; }

        decimal? PercentagePositionCancelled { get; }

        decimal? PercentageTradeCountCancelled { get; }

        bool HasBreachedRule();
    }
}