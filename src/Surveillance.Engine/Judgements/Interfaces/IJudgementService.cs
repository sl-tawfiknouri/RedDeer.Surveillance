using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IJudgementService
    {
        void Judgement(IHighProfitJudgementContext judgementContext);
        void Judgement(ICancelledOrderJudgement cancelledOrder);
        void Judgement(IHighVolumeJudgement highVolume);
        void Judgement(ILayeringJudgement layering);
        void Judgement(IMarkingTheCloseJudgement markingTheClose);
        void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
        void Judgement(IRampingJudgement ramping);
        void Judgement(ISpoofingJudgement spoofing);
        void PassJudgement();
    }
}