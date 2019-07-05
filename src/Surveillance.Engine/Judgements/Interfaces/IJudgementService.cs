using Domain.Surveillance.Judgements.Equity;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IJudgementService
    {
        void Judgement(HighProfitJudgement highProfit);
        void Judgement(CancelledOrderJudgement cancelledOrder);
        void Judgement(HighVolumeJudgement highVolume);
        void Judgement(LayeringJudgement layering);
        void Judgement(MarkingTheCloseJudgement markingTheClose);
        void Judgement(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
        void Judgement(RampingJudgement ramping);
        void Judgement(SpoofingJudgement spoofing);
    }
}