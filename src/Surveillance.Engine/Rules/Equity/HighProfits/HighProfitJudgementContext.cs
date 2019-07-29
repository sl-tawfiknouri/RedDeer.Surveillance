using Domain.Core.Financial.Money;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;
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
            bool projectToAlert)
        {
            Judgement = judgement;
            ProjectToAlert = projectToAlert;
        }

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

        public IHighProfitJudgement Judgement { get; set; }
        public bool ProjectToAlert { get; set; }
        public IRuleBreachContext RuleBreachContext { get; set; }
        public IHighProfitsRuleEquitiesParameters EquitiesParameters { get; set; }
        public bool HasRelativeProfitBreach { get; set; }
        public bool HasAbsoluteProfitBreach { get; set; }
        public string AbsoluteProfitCurrency { get; set; }
        public Money? AbsoluteProfits { get; set; }
        public decimal? RelativeProfits { get; set; }
        public IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }
    }
}
