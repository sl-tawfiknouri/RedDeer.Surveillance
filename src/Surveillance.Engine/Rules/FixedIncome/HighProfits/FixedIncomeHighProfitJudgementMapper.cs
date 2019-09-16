namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The fixed income high profit judgement mapper.
    /// </summary>
    public class FixedIncomeHighProfitJudgementMapper : IFixedIncomeHighProfitJudgementMapper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighProfitJudgementMapper> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitJudgementMapper"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FixedIncomeHighProfitJudgementMapper(ILogger<FixedIncomeHighProfitJudgementMapper> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="IRuleBreach"/>.
        /// </returns>
        public IRuleBreach Map(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogInformation($"{nameof(judgementContext)} was null in map. Returning.");
                return null;
            }

            var caseTitle = "Automated Fixed Income High Profit Rule Breach Detected";
            var description = this.BuildDescription(judgementContext);

            var ruleBreach = new RuleBreach(judgementContext.RuleBreachContext, description, caseTitle);

            return ruleBreach;
        }

        /// <summary>
        /// The build description.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BuildDescription(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            var highRelativeProfitAsPercentage = Math.Round(
                judgementContext.RelativeProfits.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var highRelativeProfitAsPercentageSetByUser = Math.Round(
                judgementContext.FixedIncomeParameters.HighProfitPercentageThreshold.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var highAbsoluteProfit = Math.Round(
                judgementContext.AbsoluteProfits?.Value ?? 0,
                2,
                MidpointRounding.AwayFromZero);

            var highRelativeProfitSection = this.HighRelativeProfitText(
                judgementContext,
                highRelativeProfitAsPercentage,
                highRelativeProfitAsPercentageSetByUser);

            var highAbsoluteProfitSection = this.HighAbsoluteProfitText(judgementContext, highAbsoluteProfit);
            var highProfitExchangeRatesSection = this.HighProfitExchangeRateText(judgementContext);

            return
                $"High profit rule breach detected for {judgementContext.RuleBreachContext.Security.Name} at {judgementContext.RuleBreachContext.UniverseDateTime}.{highRelativeProfitSection}{highAbsoluteProfitSection}{highProfitExchangeRatesSection}";
        }

        /// <summary>
        /// The high absolute profit text.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <param name="absoluteProfit">
        /// The absolute profit.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string HighAbsoluteProfitText(IFixedIncomeHighProfitJudgementContext judgementContext, decimal absoluteProfit)
        {
            return judgementContext.HasAbsoluteProfitBreach
                       ? $" There was a high profit of {absoluteProfit} ({judgementContext.AbsoluteProfits.Value.Currency}) which exceeded the configured profit limit of {judgementContext.FixedIncomeParameters.HighProfitAbsoluteThreshold.GetValueOrDefault(0)}({judgementContext.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency})."
                       : string.Empty;
        }

        /// <summary>
        /// The high profit exchange rate text.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string HighProfitExchangeRateText(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            if (!judgementContext.FixedIncomeParameters.UseCurrencyConversions
                || judgementContext.ExchangeRateProfits == null)
            {
                return string.Empty;
            }

            if (string.Equals(
                judgementContext.ExchangeRateProfits.FixedCurrency.Code,
                judgementContext.ExchangeRateProfits.VariableCurrency.Code,
                StringComparison.InvariantCultureIgnoreCase))
            {
                this.logger.LogError(
                    $"Fixed Income High Profits had two equal currencies when generating WER text {judgementContext.ExchangeRateProfits.FixedCurrency.Code} and {judgementContext.ExchangeRateProfits.VariableCurrency.Code}");
                return string.Empty;
            }

            var absAmount = Math.Round(
                judgementContext.ExchangeRateProfits.AbsoluteAmountDueToWer(),
                2,
                MidpointRounding.AwayFromZero);

            var costWer = Math.Round(
                judgementContext.ExchangeRateProfits.PositionCostWer,
                2,
                MidpointRounding.AwayFromZero);

            var revenueWer = Math.Round(
                judgementContext.ExchangeRateProfits.PositionRevenueWer,
                2,
                MidpointRounding.AwayFromZero);

            var relativePercentage = Math.Round(
                judgementContext.ExchangeRateProfits.RelativePercentageDueToWer() * 100,
                2,
                MidpointRounding.AwayFromZero);

            if (revenueWer == 0 || costWer == 0)
            {
                return string.Empty;
            }

            return
                $" The position was acquired with a currency conversion between ({judgementContext.ExchangeRateProfits.FixedCurrency.Code}/{judgementContext.ExchangeRateProfits.VariableCurrency.Code}) rate at a weighted exchange rate of {costWer} and sold at a weighted exchange rate of {revenueWer}. The impact on profits from exchange rate movements was {relativePercentage}% and the absolute amount of profits due to exchange rates is ({judgementContext.AbsoluteProfitCurrency}) {absAmount}.";
        }

        /// <summary>
        /// The high relative profit text.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <param name="highRelativeProfitAsPercentage">
        /// The high relative profit as percentage.
        /// </param>
        /// <param name="highRelativeProfitAsPercentageSetByUser">
        /// The high relative profit as percentage set by user.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string HighRelativeProfitText(
            IFixedIncomeHighProfitJudgementContext judgementContext,
            decimal highRelativeProfitAsPercentage,
            decimal highRelativeProfitAsPercentageSetByUser)
        {
            return judgementContext.HasRelativeProfitBreach
                       ? $" There was a high profit ratio of {highRelativeProfitAsPercentage}% which exceeded the configured high profit ratio percentage threshold of {highRelativeProfitAsPercentageSetByUser}%."
                       : string.Empty;
        }
    }
}