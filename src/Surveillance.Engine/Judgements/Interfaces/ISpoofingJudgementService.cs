namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface ISpoofingJudgementService
    {
        void Judgement(ISpoofingJudgement spoofing);
    }
}