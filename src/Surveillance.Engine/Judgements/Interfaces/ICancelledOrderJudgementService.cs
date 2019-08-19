namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface ICancelledOrderJudgementService
    {
        void Judgement(ICancelledOrderJudgement cancelledOrder);
    }
}