namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface IRampingJudgementService
    {
        void Judgement(IRampingJudgement ramping);
    }
}