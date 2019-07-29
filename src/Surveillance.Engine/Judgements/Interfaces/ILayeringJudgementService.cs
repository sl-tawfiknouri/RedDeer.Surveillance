using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface ILayeringJudgementService
    {
        void Judgement(ILayeringJudgement layering);
    }
}
