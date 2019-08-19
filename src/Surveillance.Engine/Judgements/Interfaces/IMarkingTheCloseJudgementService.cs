namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface IMarkingTheCloseJudgementService
    {
        void Judgement(IMarkingTheCloseJudgement markingTheClose);
    }
}