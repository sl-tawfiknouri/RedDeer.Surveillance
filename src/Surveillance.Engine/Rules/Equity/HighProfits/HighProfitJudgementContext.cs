using Domain.Core.Financial.Money;
using Domain.Surveillance.Judgement.Equity;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    public class HighProfitJudgementContext : IHighProfitJudgementContext
    {
        public HighProfitJudgementContext(
            HighProfitJudgement judgement,
            bool projectToAlert,
            IRuleBreachContext ruleBreachContext,
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            Money? absoluteProfits,
            string absoluteProfitCurrency,
            decimal? relativeProfits,
            bool hasAbsoluteProfitBreach,
            bool hasRelativeProfitBreach,
            IExchangeRateProfitBreakdown profitBreakdown)
        {
            Judgement = judgement;
            ProjectToAlert = projectToAlert;
            RuleBreachContext = ruleBreachContext;
            EquitiesParameters = equitiesParameters;
            AbsoluteProfits = absoluteProfits;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            RelativeProfits = relativeProfits;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            ExchangeRateProfits = profitBreakdown;
        }

        public HighProfitJudgement Judgement { get; }
        public bool ProjectToAlert { get; }
        public IRuleBreachContext RuleBreachContext { get; }
        public IHighProfitsRuleEquitiesParameters EquitiesParameters { get; }
        public bool HasRelativeProfitBreach { get; }
        public bool HasAbsoluteProfitBreach { get; }
        public string AbsoluteProfitCurrency { get; }
        public Money? AbsoluteProfits { get; }
        public decimal? RelativeProfits { get; }
        public IExchangeRateProfitBreakdown ExchangeRateProfits { get; }
    }
}
