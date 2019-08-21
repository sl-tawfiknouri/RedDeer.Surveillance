namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface ILayeringJudgementService
    {
        void Judgement(ILayeringJudgement layering);
    }
}