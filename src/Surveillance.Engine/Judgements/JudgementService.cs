namespace Surveillance.Engine.Rules.Judgements
{
    using System;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;

    /// <summary>
    /// The judgement service.
    /// Contains processing for rule analysis outcomes
    /// </summary>
    public class JudgementService : IJudgementService
    {
        /// <summary>
        /// The equity high profit judgement mapper.
        /// </summary>
        private readonly IHighProfitJudgementMapper equityHighProfitJudgementMapper;

        /// <summary>
        /// The fixed income high profit judgement mapper.
        /// </summary>
        private readonly IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private readonly IJudgementRepository judgementRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<JudgementService> logger;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private readonly IRuleViolationService ruleViolationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JudgementService"/> class.
        /// </summary>
        /// <param name="judgementRepository">
        /// The judgement repository.
        /// </param>
        /// <param name="ruleViolationService">
        /// The rule violation service.
        /// </param>
        /// <param name="equityHighProfitJudgementMapper">
        /// The high profit judgement mapper.
        /// </param>
        /// <param name="fixedIncomeHighProfitJudgementMapper">
        /// The fixed income high profit judgement mapper
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public JudgementService(
            IJudgementRepository judgementRepository,
            IRuleViolationService ruleViolationService,
            IHighProfitJudgementMapper equityHighProfitJudgementMapper,
            IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            this.judgementRepository =
                judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));
            this.equityHighProfitJudgementMapper =
                equityHighProfitJudgementMapper ?? throw new ArgumentNullException(nameof(equityHighProfitJudgementMapper));
            this.fixedIncomeHighProfitJudgementMapper =
                fixedIncomeHighProfitJudgementMapper ?? throw new ArgumentNullException(nameof(fixedIncomeHighProfitJudgementMapper));
            this.ruleViolationService =
                ruleViolationService ?? throw new ArgumentNullException(nameof(ruleViolationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The judgement of a high profit analysis context.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        public void Judgement(IHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogError("Equity High Profit Judgement was null");
                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                this.logger?.LogInformation(
                    "Equity High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            this.judgementRepository.Save(judgementContext.Judgement);

            if (!judgementContext.ProjectToAlert)
            {
                return;
            }

            var projectedBreach = this.equityHighProfitJudgementMapper.Map(judgementContext);
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogError("Fixed Income High Profit Judgement was null");
                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                this.logger?.LogInformation(
                    "Fixed Income High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            this.judgementRepository.Save(judgementContext.Judgement);

            if (!judgementContext.ProjectToAlert)
            {
                return;
            }

            var projectedBreach = this.fixedIncomeHighProfitJudgementMapper.Map(judgementContext);
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for a cancelled order - auto save as alert.
        /// </summary>
        /// <param name="cancelledOrder">
        /// The cancelled order.
        /// </param>
        public void Judgement(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                this.logger?.LogError("Cancelled Order Judgement was null");
                return;
            }

            this.judgementRepository.Save(cancelledOrder);

            // judgement is also a rule breach
            var projectedBreach = (ICancelledOrderRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for high volume - auto save as alert.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        public void Judgement(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                this.logger?.LogError("High Volume Judgement was null");
                return;
            }

            this.judgementRepository.Save(highVolume);

            // judgement is also a rule breach
            var projectedBreach = (IHighVolumeRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for layering - auto save as alert.
        /// </summary>
        /// <param name="layering">
        /// The layering.
        /// </param>
        public void Judgement(ILayeringJudgement layering)
        {
            if (layering == null)
            {
                this.logger?.LogError("Layering Judgement was null");
                return;
            }

            this.judgementRepository.Save(layering);

            // judgement is also a rule breach
            var projectedBreach = (ILayeringRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for marking the close - auto save as alert.
        /// </summary>
        /// <param name="markingTheClose">
        /// The marking the close.
        /// </param>
        public void Judgement(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                this.logger?.LogError("Marking The Close Judgement was null");
                return;
            }

            this.judgementRepository.Save(markingTheClose);

            // judgement is also a rule breach
            var projectedBreach = (IMarkingTheCloseBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for placing orders with no intent to execute - auto save as alert.
        /// </summary>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        public void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                this.logger?.LogError("Placing Orders Judgement was null");
                return;
            }

            this.judgementRepository.Save(placingOrders);

            // judgement is also a rule breach
            var projectedBreach = (IPlacingOrdersWithNoIntentToExecuteRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for ramping - auto save as alert.
        /// </summary>
        /// <param name="ramping">
        /// The ramping.
        /// </param>
        public void Judgement(IRampingJudgement ramping)
        {
            if (ramping == null)
            {
                this.logger?.LogError("Ramping Judgement was null");
                return;
            }

            this.judgementRepository.Save(ramping);

            // judgement is also a rule breach
            var projectedBreach = (IRampingRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for spoofing - auto save as alert.
        /// </summary>
        /// <param name="spoofing">
        /// The spoofing.
        /// </param>
        public void Judgement(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                this.logger?.LogError("Spoofing Judgement was null");
                return;
            }

            this.judgementRepository.Save(spoofing);

            // judgement is also a rule breach
            var projectedBreach = (ISpoofingRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The pass judgement call to ensure the rule violations service runs against its cache of judgements.
        /// </summary>
        public void PassJudgement()
        {
            this.ruleViolationService.ProcessRuleViolationCache();
        }
    }
}