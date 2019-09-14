using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Equities
{
    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.Equity;
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

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
            this.Judgement = judgement;
            this.ProjectToAlert = projectToAlert;
            this.RuleBreachContext = ruleBreachContext;
            this.EquitiesParameters = equitiesParameters;
            this.HasRelativeProfitBreach = hasRelativeProfitBreach;
            this.HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            this.AbsoluteProfitCurrency = absoluteProfitCurrency;
            this.AbsoluteProfits = absoluteProfits;
            this.RelativeProfits = relativeProfits;
            this.ExchangeRateProfits = exchangeRateProfits;
        }

        public string AbsoluteProfitCurrency { get; set; }

        public Money? AbsoluteProfits { get; set; }

        public IHighProfitsRuleEquitiesParameters EquitiesParameters { get; set; }

        public IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }

        public bool HasAbsoluteProfitBreach { get; set; }

        public bool HasRelativeProfitBreach { get; set; }

        public IHighProfitJudgement Judgement { get; set; }

        public bool ProjectToAlert { get; set; }

        public decimal? RelativeProfits { get; set; }

        public IRuleBreachContext RuleBreachContext { get; set; }
    }
}