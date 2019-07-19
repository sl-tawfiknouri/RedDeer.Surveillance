using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface ISpoofingJudgementService
    {
        void Judgement(ISpoofingJudgement spoofing);
    }
}
