using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IPlacingOrdersJudgementService
    {
        void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
    }
}
