namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.Equity;
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public class HighProfitJudgementContext : IHighProfitJudgementContext
    {
        public HighProfitJudgementContext(HighProfitJudgement judgement, bool projectToAlert)
        {
            this.Judgement = judgement;
            this.ProjectToAlert = projectToAlert;
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
            this.Judgement = judgement;
            this.ProjectToAlert = projectToAlert;
            this.RuleBreachContext = ruleBreachContext;
            this.EquitiesParameters = equitiesParameters;
            this.AbsoluteProfits = absoluteProfits;
            this.AbsoluteProfitCurrency = absoluteProfitCurrency;
            this.RelativeProfits = relativeProfits;
            this.HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            this.HasRelativeProfitBreach = hasRelativeProfitBreach;
            this.ExchangeRateProfits = profitBreakdown;
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