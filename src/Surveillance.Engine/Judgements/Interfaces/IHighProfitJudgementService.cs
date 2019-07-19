using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IHighProfitJudgementService
    {
        void Judgement(IHighProfitJudgementContext judgementContext);
    }
}
