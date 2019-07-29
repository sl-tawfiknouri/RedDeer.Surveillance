using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface ICancelledOrderJudgementService
    {
        void Judgement(ICancelledOrderJudgement cancelledOrder);
    }
}
