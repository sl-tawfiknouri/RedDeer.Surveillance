using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IMarkingTheCloseJudgementService
    {
        void Judgement(IMarkingTheCloseJudgement markingTheClose);
    }
}
