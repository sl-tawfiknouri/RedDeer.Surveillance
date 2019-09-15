namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IFixedIncomeHighProfitJudgementMapper
    {
        IRuleBreach Map(IFixedIncomeHighProfitJudgementContext judgementContext);
    }
}