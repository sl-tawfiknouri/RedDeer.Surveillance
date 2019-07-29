using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IRampingJudgementService
    {
        void Judgement(IRampingJudgement ramping);
    }
}
