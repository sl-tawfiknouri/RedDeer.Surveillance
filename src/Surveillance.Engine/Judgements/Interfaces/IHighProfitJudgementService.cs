namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;

    public interface IHighProfitJudgementService
    {
        void Judgement(IHighProfitJudgementContext judgementContext);
    }
}