using Domain.Surveillance.Judgement.Equity;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IJudgementService
    {
        void Judgement(IHighProfitJudgementContext judgementContext);
        void Judgement(CancelledOrderJudgement cancelledOrder);
        void Judgement(HighVolumeJudgement highVolume);
        void Judgement(LayeringJudgement layering);
        void Judgement(MarkingTheCloseJudgement markingTheClose);
        void Judgement(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
        void Judgement(RampingJudgement ramping);
        void Judgement(SpoofingJudgement spoofing);
    }
}