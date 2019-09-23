namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface IPlacingOrdersJudgementService
    {
        void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
    }
}