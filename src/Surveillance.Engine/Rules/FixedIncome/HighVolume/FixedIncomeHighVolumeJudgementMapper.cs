namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The fixed income high profit judgement mapper.
    /// </summary>
    public class FixedIncomeHighVolumeJudgementMapper : IFixedIncomeHighVolumeJudgementMapper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeJudgementMapper> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeJudgementMapper"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FixedIncomeHighVolumeJudgementMapper(ILogger<FixedIncomeHighVolumeJudgementMapper> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The mapping from judgement to rule breach .
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="IRuleBreach"/>.
        /// </returns>
        public IRuleBreach MapContextToBreach(IFixedIncomeHighVolumeJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogInformation($"{nameof(judgementContext)} was null in map. Returning.");

                return null;
            }

            var issuanceTitleSegment = judgementContext.IsIssuanceBreach ? " (%) issuance" : string.Empty;
            var caseTitle = $"Automated Fixed Income High Volume{issuanceTitleSegment} Rule Breach Detected";
            var description = this.BuildDescription(judgementContext);

            var ruleBreach = new RuleBreach(judgementContext.RuleBreachContext, description, caseTitle);

            return ruleBreach;
        }

        /// <summary>
        /// The build description functionality for a case.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string BuildDescription(IFixedIncomeHighVolumeJudgementContext judgementContext)
        {
            var issuanceTitleSegment = judgementContext.IsIssuanceBreach ? " (%) issuance" : string.Empty;
            var description = $"Fixed Income High Volume{issuanceTitleSegment} rule breach detected for {judgementContext.Security?.Name}.";

            var dailyDescription = this.BuildDailyBreachDescription(judgementContext);
            var windowDescription = this.BuildWindowBreachDescription(judgementContext);

            description = $"{description}{dailyDescription}{windowDescription}";

            return description;
        }

        /// <summary>
        /// The build daily breach description.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BuildDailyBreachDescription(IFixedIncomeHighVolumeJudgementContext judgementContext)
        {
            if (!judgementContext.Judgement.DailyAnalysisAnalysis.VolumeBreach)
            {
                return string.Empty;
            }

            var dailyPercentage = Math.Ceiling(judgementContext.FixedIncomeParameters.FixedIncomeHighVolumePercentageDaily.GetValueOrDefault(0) * 100);
            var dailyBreachPercentage = Math.Ceiling(judgementContext.Judgement.DailyAnalysisAnalysis.VolumeTradedPercentage.GetValueOrDefault(0) * 100);

            var venueDailyDescription =
                judgementContext.Venue != null
                    ? $" at the venue ({judgementContext.Venue?.MarketIdentifierCode}) {judgementContext.Venue?.Name}"
                    : string.Empty;

            var dailyDescription = $" Percentage of daily volume breach has occured. A daily volume limit of {dailyPercentage.ToString("0.##")}% was exceeded by trading {dailyBreachPercentage.ToString("0.##")}% of daily volume{venueDailyDescription}. {judgementContext.TotalOrdersTradedInWindow.ToString("0.##")} volume was the allocated fill against a breach threshold volume of {judgementContext.Judgement.DailyAnalysisAnalysis.VolumeThresholdAmount.GetValueOrDefault(0).ToString("0.##")}.";

            return dailyDescription;
        }

        /// <summary>
        /// The build window breach description.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BuildWindowBreachDescription(IFixedIncomeHighVolumeJudgementContext judgementContext)
        {
            if (!judgementContext.Judgement.WindowAnalysisAnalysis.VolumeBreach)
            {
                return string.Empty;
            }

            var windowPercentage = Math.Ceiling(judgementContext.FixedIncomeParameters.FixedIncomeHighVolumePercentageWindow.GetValueOrDefault(0) * 100);
            var windowBreachPercentage = Math.Ceiling(judgementContext.Judgement.WindowAnalysisAnalysis.VolumeTradedPercentage.GetValueOrDefault(0) * 100);

            var venueWindowDescription =
                judgementContext.Venue != null
                ? $" at the venue ({judgementContext.Venue?.MarketIdentifierCode}) {judgementContext.Venue?.Name}"
                : string.Empty;

            var windowDescription = $" Percentage of window volume breach has occured. A window volume limit of {windowPercentage.ToString("0.##")}% was exceeded by trading {windowBreachPercentage.ToString("0.##")}% of window volume within the window of {judgementContext.FixedIncomeParameters.Windows.BackwardWindowSize.TotalMinutes} minutes{venueWindowDescription}. {judgementContext.TotalOrdersTradedInWindow.ToString("0.##")} volume was the allocated fill against a breach threshold volume of {judgementContext.Judgement.WindowAnalysisAnalysis.VolumeThresholdAmount.GetValueOrDefault(0).ToString("0.##")}.";

            return windowDescription;
        }

    }
}