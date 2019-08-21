namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IHighProfitJudgementMapper
    {
        IRuleBreach Map(IHighProfitJudgementContext judgementContext);
    }
}