using Domain.Core.Financial.Money;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Equities
{
    public class HighProfitJudgementContext : IHighProfitJudgementContext
    {
        public HighProfitJudgementContext(
            HighProfitJudgement judgement,
            bool projectToAlert,
            IRuleBreachContext ruleBreachContext,
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            bool hasRelativeProfitBreach,
            bool hasAbsoluteProfitBreach, 
            string absoluteProfitCurrency, 
            Money? absoluteProfits,
            decimal? relativeProfits,
            IExchangeRateProfitBreakdown exchangeRateProfits)
        {
            Judgement = judgement;
            ProjectToAlert = projectToAlert;
            RuleBreachContext = ruleBreachContext;
            EquitiesParameters = equitiesParameters;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            AbsoluteProfits = absoluteProfits;
            RelativeProfits = relativeProfits;
            ExchangeRateProfits = exchangeRateProfits;
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
